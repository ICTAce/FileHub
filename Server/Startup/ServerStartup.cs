// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Features.Common.Behaviors;

namespace ICTAce.FileHub.Startup;

public class ServerStartup : IServerStartup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // not implemented
    }

    public void ConfigureMvc(IMvcBuilder mvcBuilder)
    {
        // not implemented
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Register MediatR with pipeline behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServerStartup).Assembly);

            // Add pipeline behaviors (order matters!)
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // Register DbContext factory
        services.AddDbContextFactory<Context>(opt => { }, ServiceLifetime.Transient);
    }
}
