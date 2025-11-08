using BackendAspNetCore.Data;
using BackendAspNetCore.DependencyInjectionRegister;
using Microsoft.EntityFrameworkCore;
using BackendAspNetCore.Dtos.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using FluentValidation.AspNetCore;
using FluentValidation;
using BackendAspNetCore.Mappers;
using ZiggyCreatures.Caching.Fusion; // FusionCache

var builder = WebApplication.CreateBuilder(args);

// Controllers + GLOBAL "api" prefix via convention
builder.Services.AddControllers(options =>
{
    options.Conventions.Insert(0, new RoutePrefixConvention("api"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EF Core DbContext
// register ms-sql
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql =>
        {
            sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            sql.CommandTimeout(180);
        });
});

// bind interface and concrete class
builder.Services.BindApplicationServices();

// API versioning
builder.Services.AddApiVersioning(version =>
{
    version.DefaultApiVersion = new ApiVersion(1, 0);
    version.AssumeDefaultVersionWhenUnspecified = true;
    version.ReportApiVersions = true;
});

// For Swagger grouping by version
builder.Services.AddVersionedApiExplorer(version =>
{
    version.GroupNameFormat = "'v'VVV";           // v1, v2, v2.1
    version.SubstituteApiVersionInUrl = true;     // replaces {version:apiVersion}
});
// Wire Swagger options per version
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddCors(option => option.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5000", "http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials())
    );

// fluent validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ✅ Register AutoMapper (this scans the assembly for all Profile classes)
builder.Services.AddAutoMapper(typeof(SensorProfile).Assembly);

// in-proc memory cache
builder.Services.AddMemoryCache();
// FusionCache default instance (so can inject IFusionCache)
builder.Services.AddFusionCache();

var app = builder.Build();

// Apply pending migrations and create DB if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // <-- creates DB if missing, then applies migrations
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    // Apply migrations & seed demo data (dev only)
    await SensorSeed.EnsureSeedAsync(app.Services, total: 10);
    
    app.UseSwagger();
    // Build a Swagger UI tab per API version
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                                    desc.GroupName.ToUpperInvariant());
        }
    });
}

// Global exception handler
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

// This line keeps the process alive:
app.Run();