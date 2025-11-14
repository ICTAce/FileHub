namespace ICTAce.FileHub.Services;

public interface ICategoryService
{
    Task<List<Models.Category>> GetCategoriesAsync(int ModuleId);
    Task<List<Models.Category>> GetCategoriesByParentAsync(int? ParentCategoryId, int ModuleId);
    Task<Models.Category> GetCategoryAsync(int CategoryId, int ModuleId);
    Task<Models.Category> AddCategoryAsync(Models.Category Category);
    Task<Models.Category> UpdateCategoryAsync(Models.Category Category);
    Task DeleteCategoryAsync(int CategoryId, int ModuleId);
}

public class CategoryService : ServiceBase, ICategoryService
{
    public CategoryService(HttpClient http, SiteState siteState) : base(http, siteState) { }

    private string Apiurl => CreateApiUrl("Category");

    public async Task<List<Models.Category>> GetCategoriesAsync(int ModuleId)
    {
        List<Models.Category> Categories = await GetJsonAsync<List<Models.Category>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.Category>().ToList());
        return Categories.OrderBy(item => item.Name).ToList();
    }

    public async Task<List<Models.Category>> GetCategoriesByParentAsync(int? ParentCategoryId, int ModuleId)
    {
        string parentParam = ParentCategoryId.HasValue ? ParentCategoryId.Value.ToString() : "";
        List<Models.Category> Categories = await GetJsonAsync<List<Models.Category>>(
            CreateAuthorizationPolicyUrl($"{Apiurl}/parent?parentid={parentParam}&moduleid={ModuleId}", EntityNames.Module, ModuleId), 
            Enumerable.Empty<Models.Category>().ToList());
        return Categories.OrderBy(item => item.Name).ToList();
    }

    public async Task<Models.Category> GetCategoryAsync(int CategoryId, int ModuleId)
    {
        return await GetJsonAsync<Models.Category>(CreateAuthorizationPolicyUrl($"{Apiurl}/{CategoryId}/{ModuleId}", EntityNames.Module, ModuleId));
    }

    public async Task<Models.Category> AddCategoryAsync(Models.Category Category)
    {
        return await PostJsonAsync<Models.Category>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, Category.ModuleId), Category);
    }

    public async Task<Models.Category> UpdateCategoryAsync(Models.Category Category)
    {
        return await PutJsonAsync<Models.Category>(CreateAuthorizationPolicyUrl($"{Apiurl}/{Category.CategoryId}", EntityNames.Module, Category.ModuleId), Category);
    }

    public async Task DeleteCategoryAsync(int CategoryId, int ModuleId)
    {
        await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{CategoryId}/{ModuleId}", EntityNames.Module, ModuleId));
    }
}
