// Licensed to ICTAce under the MIT license.

using Radzen;

namespace ICTAce.FileHub.Client.Startup;

public class ClientStartup : IClientStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        if (!services.Any(s => s.ServiceType == typeof(ISampleModuleService)))
        {
            services.AddScoped<ISampleModuleService, SampleModuleService>();
        }

        if (!services.Any(s => s.ServiceType == typeof(ICategoryService)))
        {
            services.AddScoped<ICategoryService, CategoryService>();
        }

        if (!services.Any(s => s.ServiceType == typeof(Services.IFileService)))
        {
            services.AddScoped<Services.IFileService, Services.FileService>();
        }

        services.AddRadzenComponents();
    }
}
