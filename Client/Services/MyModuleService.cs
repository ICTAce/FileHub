namespace ICTAce.FileHub.Services;

public interface IMyModuleService
{
    Task<List<Shared.Models.MyModule>> GetMyModulesAsync(int ModuleId);

    Task<Shared.Models.MyModule> GetMyModuleAsync(int MyModuleId, int ModuleId);

    Task<Shared.Models.MyModule> AddMyModuleAsync(Shared.Models.MyModule MyModule);

    Task<Shared.Models.MyModule> UpdateMyModuleAsync(Shared.Models.MyModule MyModule);

    Task DeleteMyModuleAsync(int MyModuleId, int ModuleId);
}

public class MyModuleService : ServiceBase, IMyModuleService
{
    public MyModuleService(HttpClient http, SiteState siteState) : base(http, siteState) { }

    private string Apiurl => CreateApiUrl("MyModule");

    public async Task<List<Shared.Models.MyModule>> GetMyModulesAsync(int ModuleId)
    {
        List<Shared.Models.MyModule> Tasks = await GetJsonAsync<List<Shared.Models.MyModule>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Shared.Models.MyModule>().ToList());
        return Tasks.OrderBy(item => item.Name).ToList();
    }

    public async Task<Shared.Models.MyModule> GetMyModuleAsync(int MyModuleId, int ModuleId)
    {
        return await GetJsonAsync<Shared.Models.MyModule>(CreateAuthorizationPolicyUrl($"{Apiurl}/{MyModuleId}/{ModuleId}", EntityNames.Module, ModuleId));
    }

    public async Task<Shared.Models.MyModule> AddMyModuleAsync(Shared.Models.MyModule MyModule)
    {
        return await PostJsonAsync<Shared.Models.MyModule>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, MyModule.ModuleId), MyModule);
    }

    public async Task<Shared.Models.MyModule> UpdateMyModuleAsync(Shared.Models.MyModule MyModule)
    {
        return await PutJsonAsync<Shared.Models.MyModule>(CreateAuthorizationPolicyUrl($"{Apiurl}/{MyModule.MyModuleId}", EntityNames.Module, MyModule.ModuleId), MyModule);
    }

    public async Task DeleteMyModuleAsync(int MyModuleId, int ModuleId)
    {
        await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{MyModuleId}/{ModuleId}", EntityNames.Module, ModuleId));
    }
}
