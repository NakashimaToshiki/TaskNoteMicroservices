using Microsoft.Extensions.DependencyInjection;

namespace Job.Entity.FrameworkCore.Sessions;

public static class JobDbSessionsServiceCollectionExtentions
{
    public static IServiceCollection AddDbSessionServices(this IServiceCollection services) =>
        services
        .AddSingleton<JobSession>()
        ;
}
