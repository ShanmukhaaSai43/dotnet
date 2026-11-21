using System.Text;
using EmployeeManagement.API.Configuration;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.API.Services.Lifetimes;
using EmployeeManagement.Application;
using EmployeeManagement.Application.Configuration;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers
builder.Services.AddControllers();

// 2. Strongly-Typed Configuration via IOptions Pattern
builder.Services.Configure<PayrollSettings>(builder.Configuration.GetSection(PayrollSettings.SectionName));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// 3. JWT Authentication & Token Validation
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Role-Based Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy("RequireManagerOrAdmin", policy => policy.RequireRole(Roles.Admin, Roles.Manager));
    options.AddPolicy("RequireEmployee", policy => policy.RequireRole(Roles.Employee, Roles.Manager, Roles.Admin));
});

// 5. Add EndpointsApiExplorer & Swagger with JWT Bearer Authentication UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Employee & Department Management API",
        Version = "v1",
        Description = "ASP.NET Core 8.0 REST API with EF Core, SQL Server, Layered Architecture, FluentValidation, RFC 7807, and JWT Bearer Security (Gate 2).",
        Contact = new OpenApiContact
        {
            Name = "Shanmukha-Sai",
            Email = "kalugurisai2002@gmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token format: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 6. Register Dependency Injection Service Lifetimes
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

// 7. Configure the HTTP Request Pipeline (Order is critical!)
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

// 3) Authentication MUST run before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
