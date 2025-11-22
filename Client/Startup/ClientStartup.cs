// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Startup;

public class ClientStartup : IClientStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        if (!services.Any(s => s.ServiceType == typeof(IMyModuleService)))
        {
            services.AddScoped<IMyModuleService, MyModuleService>();
        }

        if (!services.Any(s => s.ServiceType == typeof(ICategoryService)))
        {
            services.AddScoped<ICategoryService, CategoryService>();
        }
    }
}
