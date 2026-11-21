using EmployeeManagement.API.Services.Lifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.UnitTests.DiAndMiddlewareTests;

public class ServiceLifetimeTests
{
    [Fact]
    public void TransientLifetime_EachResolution_ShouldProduceDistinctInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IOperationTransient, OperationService>();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<IOperationTransient>();
        var instance2 = provider.GetRequiredService<IOperationTransient>();

        // Assert
        Assert.NotEqual(instance1.OperationId, instance2.OperationId);
        Assert.False(ReferenceEquals(instance1, instance2));
    }

    [Fact]
    public void ScopedLifetime_WithinSameScope_ShouldProduceIdenticalInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IOperationScoped, OperationService>();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var instance1 = scope.ServiceProvider.GetRequiredService<IOperationScoped>();
        var instance2 = scope.ServiceProvider.GetRequiredService<IOperationScoped>();

        // Assert (Same scope => same instance)
        Assert.Equal(instance1.OperationId, instance2.OperationId);
        Assert.True(ReferenceEquals(instance1, instance2));
    }

    [Fact]
    public void ScopedLifetime_AcrossDifferentScopes_ShouldProduceDifferentInstances()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IOperationScoped, OperationService>();
        var provider = services.BuildServiceProvider();

        IOperationScoped instanceScope1;
        IOperationScoped instanceScope2;

        // Act
        using (var scope1 = provider.CreateScope())
        {
            instanceScope1 = scope1.ServiceProvider.GetRequiredService<IOperationScoped>();
        }

        using (var scope2 = provider.CreateScope())
        {
            instanceScope2 = scope2.ServiceProvider.GetRequiredService<IOperationScoped>();
        }

        // Assert (Different scopes => different instances)
        Assert.NotEqual(instanceScope1.OperationId, instanceScope2.OperationId);
    }

    [Fact]
    public void SingletonLifetime_AcrossDifferentScopes_ShouldAlwaysProduceIdenticalInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IOperationSingleton, OperationService>();
        var provider = services.BuildServiceProvider();

        IOperationSingleton instance1;
        IOperationSingleton instance2;

        // Act
        using (var scope1 = provider.CreateScope())
        {
            instance1 = scope1.ServiceProvider.GetRequiredService<IOperationSingleton>();
        }

        using (var scope2 = provider.CreateScope())
        {
            instance2 = scope2.ServiceProvider.GetRequiredService<IOperationSingleton>();
        }

        // Assert (Singleton across entire provider => same instance)
        Assert.Equal(instance1.OperationId, instance2.OperationId);
        Assert.True(ReferenceEquals(instance1, instance2));
    }
}
