using BackendAspNetCore.Data;
using BackendAspNetCore.DependencyInjectionRegister;
using Microsoft.EntityFrameworkCore;
using BackendAspNetCore.Dtos.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using FluentValidation.AspNetCore;
using FluentValidation;
using BackendAspNetCore.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Controllers + GLOBAL "api" prefix via convention
builder.Services.AddControllers(options =>
{
    options.Conventions.Insert(0, new RoutePrefixConvention("api"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
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

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()));

// fluent validation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ✅ Register AutoMapper (this scans the assembly for all Profile classes)
builder.Services.AddAutoMapper(typeof(SensorProfile).Assembly);

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
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