using System.Reflection;
using EmployeeManagement.API.Configuration;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.API.Services.Lifetimes;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee & Department Management REST API",
        Description = "ASP.NET Core Web API with REST CRUD, Dependency Injection lifetimes, and custom logging middleware.",
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
// Transient: Fresh instance every time it is injected
builder.Services.AddTransient<IOperationTransient, OperationService>();

// Scoped: One instance created per HTTP request lifecycle
builder.Services.AddScoped<IOperationScoped, OperationService>();

// Singleton: Single instance created once for the entire application duration
builder.Services.AddSingleton<IOperationSingleton, OperationService>();

// Domain Services & Repositories
var employeeRepository = new InMemoryEmployeeRepository();
SeedInitialData(employeeRepository);

builder.Services.AddSingleton<IEmployeeRepository>(employeeRepository);
builder.Services.AddScoped<IPayrollCalculator, StandardPayrollCalculator>();
builder.Services.AddTransient<LinqQueryService>();

var app = builder.Build();

// 5. Configure the HTTP Request Pipeline (Order is critical!)
// Custom Request Logging Middleware runs earliest to capture total elapsed time and assign X-Correlation-Id
app.UseRequestLogging();

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
        address: new Address("200 Pine St", "Seattle", "WA", "98101")
    ));

    repository.Add(new FullTimeEmployee(
        id: 103,
        name: "Carol Danvers",
        email: "carol.danvers@company.com",
        departmentId: 2,
        annualSalary: 95000m,
        monthlyBenefitsAllowance: 600m,
        address: new Address("300 Cloud Way", "Austin", "TX", "78701")
    ));
}
