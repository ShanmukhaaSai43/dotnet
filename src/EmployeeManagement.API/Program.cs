using System.Reflection;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Repositories;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI with detailed metadata
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee & Department Management REST API",
        Description = "ASP.NET Core Web API implementing RESTful conventions, CRUD operations, DTOs, and OpenAPI documentation.",
        Contact = new OpenApiContact
        {
            Name = "Shanmukha-Sai",
            Email = "kalugurisai2002@gmail.com"
        }
    });
});

// Register Domain Services and Seeded In-Memory Repository (Singleton for consistent in-memory lifecycle)
var employeeRepository = new InMemoryEmployeeRepository();
SeedInitialData(employeeRepository);

builder.Services.AddSingleton<IEmployeeRepository>(employeeRepository);
builder.Services.AddSingleton<IPayrollCalculator, StandardPayrollCalculator>();
builder.Services.AddSingleton<LinqQueryService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root: http://localhost:PORT/
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
