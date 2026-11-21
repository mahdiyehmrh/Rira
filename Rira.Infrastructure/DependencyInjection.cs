using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.Interfaces;
using Rira.Infrastructure.Persistence;

namespace Rira.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
