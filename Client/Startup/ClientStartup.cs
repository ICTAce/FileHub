using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Oqtane.Services;
using ICTAce.FileHub.Services;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

namespace ICTAce.FileHub.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IMyModuleService)))
            {
                services.AddScoped<IMyModuleService, MyModuleService>();
            }

            services.AddSyncfusionBlazor();

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            SyncfusionLicenseProvider.RegisterLicense(configuration["Syncfusion:LicenseKey"]);
        }
    }
}
