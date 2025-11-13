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
        services.AddTransient<IMyModuleService, ServerMyModuleService>();
        services.AddDbContextFactory<Context>(opt => { }, ServiceLifetime.Transient);
    }
}
