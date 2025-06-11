using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Repository;
using RecipeShare.Services;

var builder = WebApplication.CreateBuilder(args);

// Add environment variable substitution for connection strings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    connectionString = connectionString
        .Replace("${DB_SERVER}", Environment.GetEnvironmentVariable("DB_SERVER") ?? "")
        .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "")
        .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "")
        .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "");
}

// Validate connection string
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured. Please check your app settings."
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

// Add SPA static files configuration
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

    // Force HTTPS in production
    app.UseHttpsRedirection();
}

// Add Static Files and SPA configuration
app.UseStaticFiles();
app.UseSpaStaticFiles();

// Configure static files to serve from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseCors("AllowAngularApp");
app.UseAuthorization();

// Map API controllers first
app.MapControllers();

// Configure SPA routing - only for non-API routes
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/api"),
    appBuilder =>
    {
        appBuilder.UseSpa(spa =>
        {
            spa.Options.SourcePath = "../RecipeShareAngularApp";
            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            };

            if (app.Environment.IsDevelopment())
            {
                // In development, proxy to the Angular dev server
                spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            }
            // In production, ensure static files are served from wwwroot
            else
            {
                spa.Options.DefaultPage = "/index.html";
            }
        });
    }
);

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting database initialization...");

        var context = services.GetRequiredService<ApplicationDbContext>();

        // Set a timeout for database operations
        var timeout = TimeSpan.FromMinutes(2);
        using var cts = new CancellationTokenSource(timeout);

        // First, ensure the database exists
        logger.LogInformation("Ensuring database exists...");
        await context.Database.EnsureCreatedAsync(cts.Token);
        logger.LogInformation("Database creation/verification completed.");

        // Then try to apply migrations
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
