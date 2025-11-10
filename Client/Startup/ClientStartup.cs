using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Oqtane.Services;
using ICTAce.FileHub.Services;

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
        }
    }
}
