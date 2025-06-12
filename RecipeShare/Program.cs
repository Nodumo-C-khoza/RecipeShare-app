using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Repository;
using RecipeShare.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    var logConnectionString = connectionString
        .Replace("${DB_PASSWORD}", "***")
        .Replace("${DB_USER}", "***");
    Console.WriteLine($"Initial connection string template: {logConnectionString}");
}

if (!string.IsNullOrEmpty(connectionString))
{
    var dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

    Console.WriteLine($"DB_SERVER: {dbServer}");
    Console.WriteLine($"DB_NAME: {dbName}");
    Console.WriteLine($"DB_USER: {dbUser}");
    Console.WriteLine($"DB_PASSWORD: {(string.IsNullOrEmpty(dbPassword) ? "NOT SET" : "SET")}");

    connectionString = connectionString
        .Replace("${DB_SERVER}", dbServer ?? "")
        .Replace("${DB_NAME}", dbName ?? "")
        .Replace("${DB_USER}", dbUser ?? "")
        .Replace("${DB_PASSWORD}", dbPassword ?? "");

    var finalLogConnectionString = connectionString;
    if (!string.IsNullOrEmpty(dbPassword))
    {
        finalLogConnectionString = connectionString.Replace(dbPassword, "***");
    }
    Console.WriteLine($"Final connection string: {finalLogConnectionString}");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured. Please check your app settings."
    );
}

if (connectionString.Contains("${"))
{
    throw new InvalidOperationException(
        $"Connection string contains unresolved placeholders: {connectionString}"
    );
}

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Add Repository and Service
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngularApp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:4200", "https://recipeshare.azurewebsites.net")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

// Configure EasyCaching
builder.Services.AddEasyCaching(options =>
{
    options.UseInMemory("default");
});

// Add SPA static files configuration - we'll configure this dynamically later
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Production settings
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecipeShare API v1");
        c.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseSpaStaticFiles();

app.UseCors("AllowAngularApp");
app.UseAuthorization();

// Map API controllers first
app.MapControllers();

// Configure SPA routing - only for non-API routes
app.UseWhen(
    context =>
        !context.Request.Path.StartsWithSegments("/api")
        && !context.Request.Path.StartsWithSegments("/swagger"),
    appBuilder =>
    {
        appBuilder.UseSpa(spa =>
        {
            spa.Options.SourcePath = "../RecipeShareAngularApp";

            if (app.Environment.IsDevelopment())
            {
                // In development, proxy to the Angular dev server
                spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            }
            else
            {
                // In production, serve from wwwroot
                spa.Options.DefaultPage = "/index.html";
                spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(app.Environment.ContentRootPath, "wwwroot")
                    )
                };

                Console.WriteLine("SPA configured for production - serving from wwwroot");

                // Log if index.html exists
                var indexPath = Path.Combine(
                    app.Environment.ContentRootPath,
                    "wwwroot",
                    "index.html"
                );
                Console.WriteLine($"Looking for index.html at: {indexPath}");
                Console.WriteLine($"Index.html exists: {File.Exists(indexPath)}");
            }
        });
    }
);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting database initialization...");

        var context = services.GetRequiredService<ApplicationDbContext>();

        var timeout = TimeSpan.FromMinutes(2);
        using var cts = new CancellationTokenSource(timeout);

        logger.LogInformation("Ensuring database exists...");
        await context.Database.EnsureCreatedAsync(cts.Token);
        logger.LogInformation("Database creation/verification completed.");

        try
        {
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync(cts.Token);
            logger.LogInformation("Database migrations completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "An error occurred while applying database migrations. This might be normal if it's the first run or if migrations are already applied."
            );
        }

        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "A critical error occurred while initializing the database. Application will continue to start but may not function properly."
        );
        // Don't throw here - we want the app to start even if database initialization fails
    }
}

app.Run();

