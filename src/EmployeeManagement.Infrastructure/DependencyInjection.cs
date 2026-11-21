using EmployeeManagement.Application.Configuration;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Contracts;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["DatabaseProvider"] ?? "SqlServer";
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost\\SQLEXPRESS;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        if (provider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<EmployeeManagementDbContext>(options =>
                options.UseInMemoryDatabase("EmployeeManagementDb"));
        }
        else
        {
            services.AddDbContext<EmployeeManagementDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(EmployeeManagementDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                    );
                }));
        }

        // Register EF Core SqlEmployeeRepository for database persistence
        services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
        services.AddScoped<SqlEmployeeRepository>();

        // Register JWT and Security infrastructure
        var jwtConfig = configuration.GetSection(JwtSettings.SectionName);
        services.Configure<JwtSettings>(options =>
        {
            options.SecretKey = jwtConfig[nameof(JwtSettings.SecretKey)] ?? options.SecretKey;
            options.Issuer = jwtConfig[nameof(JwtSettings.Issuer)] ?? options.Issuer;
            options.Audience = jwtConfig[nameof(JwtSettings.Audience)] ?? options.Audience;
            if (int.TryParse(jwtConfig[nameof(JwtSettings.ExpiryMinutes)], out var exp))
            {
                options.ExpiryMinutes = exp;
            }
        });
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
