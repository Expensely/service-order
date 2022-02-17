using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MultiCarrier.Database.Extensions;

public static class ServiceCollection
{
    public static IServiceCollection AddMultiCarrierDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime contextLifeCycle = ServiceLifetime.Scoped)
    {
        services.AddDbContext<MultiCarrierContext>(
            options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Default"));
                options.EnableSensitiveDataLogging();
                options.UseSnakeCaseNamingConvention();
            }, 
            contextLifeCycle);
        return services;
    }
}