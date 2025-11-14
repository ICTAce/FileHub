namespace ICTAce.FileHub.Manager;

public class CategoryManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
{
    private readonly ICategoryRepository _CategoryRepository;
    private readonly IDBContextDependencies _DBContextDependencies;

    public CategoryManager(ICategoryRepository CategoryRepository, IDBContextDependencies DBContextDependencies)
    {
        _CategoryRepository = CategoryRepository;
        _DBContextDependencies = DBContextDependencies;
    }

    public bool Install(Tenant tenant, string version)
    {
        return Migrate(new Context(_DBContextDependencies), tenant, MigrationType.Up);
    }

    public bool Uninstall(Tenant tenant)
    {
        return Migrate(new Context(_DBContextDependencies), tenant, MigrationType.Down);
    }

    public string ExportModule(Module module)
    {
        string content = "";
        List<Models.Category> Categories = _CategoryRepository.GetCategories(module.ModuleId).ToList();
        if (Categories != null)
        {
            content = JsonSerializer.Serialize(Categories);
        }
        return content;
    }

    public void ImportModule(Module module, string content, string version)
    {
        List<Models.Category> Categories = null;
        if (!string.IsNullOrEmpty(content))
        {
            Categories = JsonSerializer.Deserialize<List<Models.Category>>(content);
        }
        if (Categories != null)
        {
            // Import root categories first
            var rootCategories = Categories.Where(c => !c.ParentCategoryId.HasValue).ToList();
            var categoryIdMap = new Dictionary<int, int>(); // Old ID -> New ID mapping
            
            foreach(var category in rootCategories)
            {
                var oldId = category.CategoryId;
                var newCategory = _CategoryRepository.AddCategory(new Models.Category 
                { 
                    ModuleId = module.ModuleId, 
                    Name = category.Name,
                    Description = category.Description,
                    ParentCategoryId = null
                });
                categoryIdMap[oldId] = newCategory.CategoryId;
            }
            
            // Import child categories
            var childCategories = Categories.Where(c => c.ParentCategoryId.HasValue).OrderBy(c => c.ParentCategoryId).ToList();
            foreach(var category in childCategories)
            {
                var oldId = category.CategoryId;
                var newParentId = category.ParentCategoryId.HasValue && categoryIdMap.ContainsKey(category.ParentCategoryId.Value)
                    ? categoryIdMap[category.ParentCategoryId.Value]
                    : (int?)null;
                    
                var newCategory = _CategoryRepository.AddCategory(new Models.Category 
                { 
                    ModuleId = module.ModuleId, 
                    Name = category.Name,
                    Description = category.Description,
                    ParentCategoryId = newParentId
                });
                categoryIdMap[oldId] = newCategory.CategoryId;
            }
        }
    }

    public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
    {
       var searchContentList = new List<SearchContent>();

       foreach (var category in _CategoryRepository.GetCategories(pageModule.ModuleId))
       {
           if (category.ModifiedOn >= lastIndexedOn)
           {
               searchContentList.Add(new SearchContent
               {
                   EntityName = "Category",
                   EntityId = category.CategoryId.ToString(),
                   Title = category.Name,
                   Body = category.Description ?? category.Name,
                   ContentModifiedBy = category.ModifiedBy,
                   ContentModifiedOn = category.ModifiedOn
               });
           }
       }

       return Task.FromResult(searchContentList);
    }
}
