using EmployeeManagement.API.Configuration;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.API.Services.Lifetimes;
using EmployeeManagement.Application;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Add EndpointsApiExplorer & Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee & Department Management API",
        Version = "v1",
        Description = "ASP.NET Core 8.0 REST API with EF Core, SQL Server, Layered Architecture, FluentValidation, and RFC 7807 Exception Handling.",
        Contact = new OpenApiContact
        {
            Name = "Shanmukha-Sai",
            Email = "kalugurisai2002@gmail.com"
        }
    });
});

// 3. Strongly-Typed Configuration via IOptions Pattern
builder.Services.Configure<PayrollSettings>(builder.Configuration.GetSection(PayrollSettings.SectionName));

// 4. Register Dependency Injection Service Lifetimes
builder.Services.AddTransient<IOperationTransient, OperationService>();
builder.Services.AddScoped<IOperationScoped, OperationService>();
builder.Services.AddSingleton<IOperationSingleton, OperationService>();

// Domain Services
builder.Services.AddScoped<IPayrollCalculator, StandardPayrollCalculator>();
builder.Services.AddTransient<LinqQueryService>();

// Register Application & Infrastructure Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Seed Database on startup if available
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeManagementDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}
catch (Exception ex)
{
    app.Logger.LogWarning("SQL Server automatic seeding skipped: {Message}", ex.Message);
}

// 5. Configure the HTTP Request Pipeline (Order is critical!)
// 1) Custom Request Logging Middleware runs earliest to capture total elapsed time and assign X-Correlation-Id
app.UseRequestLogging();

// 2) Centralized Global Exception Handler intercepts all domain/app exceptions and formats RFC 7807 ProblemDetails
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
