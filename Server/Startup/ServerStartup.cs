// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Server.Persistence;

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
        });

        // Register DbContext factory
        services.AddDbContextFactory<MyModuleCommandContext>(opt => { }, ServiceLifetime.Transient);
        services.AddDbContextFactory<MyModuleQueryContext>(opt => { }, ServiceLifetime.Transient);
    }
}
