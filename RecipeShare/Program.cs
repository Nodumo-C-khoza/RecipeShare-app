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
    // Use memory cache
    options.UseInMemory("default");
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
app.UseSpaStaticFiles(); // Use this if you have a separate folder for SPA static files, configured via services.AddSpaStaticFiles

app.UseCors("AllowAngularApp");
app.UseAuthorization();

app.MapControllers();

// Configure SPA routing
app.UseSpa(spa =>
{
    // In production, the Angular files will be served from this directory
    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // This is mainly for dev, ensure it's handled in prod correctly
    spa.Options.SourcePath = "../RecipeShareAngularApp";
});

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
