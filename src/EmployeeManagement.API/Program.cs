using EmployeeManagement.API.Configuration;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.API.Services.Lifetimes;
using EmployeeManagement.Application;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;
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
        Description = "ASP.NET Core 8.0 REST API with Layered Architecture, FluentValidation, DI Lifetimes, and RFC 7807 Exception Handling.",
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

// Domain Services & Repositories
var employeeRepository = new InMemoryEmployeeRepository();
SeedInitialData(employeeRepository);

builder.Services.AddSingleton<IEmployeeRepository>(employeeRepository);
builder.Services.AddScoped<IPayrollCalculator, StandardPayrollCalculator>();
builder.Services.AddTransient<LinqQueryService>();

// Register Application Layer (FluentValidation + Services)
builder.Services.AddApplication();

var app = builder.Build();

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

static void SeedInitialData(IEmployeeRepository repository)
{
    repository.Add(new FullTimeEmployee(
        id: 101,
        name: "Alice Johnson",
        email: "alice.johnson@company.com",
        departmentId: 1,
        annualSalary: 120000m,
        monthlyBenefitsAllowance: 800m,
        address: new Address("100 Tech Way", "Seattle", "WA", "98101")
    ));

    repository.Add(new ContractEmployee(
        id: 102,
        name: "Bob Martin",
        email: "bob.martin@contractor.com",
        departmentId: 1,
        hourlyRate: 85m,
        hoursWorkedThisMonth: 160,
        contractEndDate: DateTime.UtcNow.AddMonths(6),
        address: new Address("200 Contract Blvd", "Portland", "OR", "97201")
    ));

    repository.Add(new FullTimeEmployee(
        id: 103,
        name: "Carol White",
        email: "carol.white@company.com",
        departmentId: 2,
        annualSalary: 85000m,
        monthlyBenefitsAllowance: 500m,
        address: new Address("300 People St", "Austin", "TX", "73301")
    ));

    repository.Add(new FullTimeEmployee(
        id: 104,
        name: "David Kim",
        email: "david.kim@company.com",
        departmentId: 1,
        annualSalary: 145000m,
        monthlyBenefitsAllowance: 1000m,
        address: new Address("400 Cloud Ave", "Bellevue", "WA", "98004")
    ));
}
