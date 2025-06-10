using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Interfaces;
using RecipeShare.Repository;
using RecipeShare.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
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
            builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
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
    configuration.RootPath = "../RecipeShareAngularApp/dist";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Static Files and SPA configuration
app.UseStaticFiles();
app.UseSpaStaticFiles();

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

            if (app.Environment.IsDevelopment())
            {
                // In development, proxy to the Angular dev server
                spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
            }
            // In production, the Angular files will be served from the configured root path
        });
    }
);

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
