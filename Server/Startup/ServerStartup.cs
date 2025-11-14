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
        // Register MediatR for Vertical Slice Architecture
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServerStartup).Assembly));
        
        // Register DbContext factory
        services.AddDbContextFactory<Context>(opt => { }, ServiceLifetime.Transient);
    }
}
