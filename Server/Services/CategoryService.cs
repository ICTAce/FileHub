namespace ICTAce.FileHub.Services;

public class ServerCategoryService : ICategoryService
{
    private readonly ICategoryRepository _CategoryRepository;
    private readonly IUserPermissions _userPermissions;
    private readonly ILogManager _logger;
    private readonly IHttpContextAccessor _accessor;
    private readonly Alias _alias;

    public ServerCategoryService(ICategoryRepository CategoryRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
    {
        _CategoryRepository = CategoryRepository;
        _userPermissions = userPermissions;
        _logger = logger;
        _accessor = accessor;
        _alias = tenantManager.GetAlias();
    }

    public Task<List<Models.Category>> GetCategoriesAsync(int ModuleId)
    {
        if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
        {
            return Task.FromResult(_CategoryRepository.GetCategories(ModuleId).ToList());
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {ModuleId}", ModuleId);
            return null;
        }
    }

    public Task<List<Models.Category>> GetCategoriesByParentAsync(int? ParentCategoryId, int ModuleId)
    {
        if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
        {
            return Task.FromResult(_CategoryRepository.GetCategoriesByParent(ParentCategoryId, ModuleId).ToList());
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {ModuleId}", ModuleId);
            return null;
        }
    }

    public Task<Models.Category> GetCategoryAsync(int CategoryId, int ModuleId)
    {
        if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
        {
            return Task.FromResult(_CategoryRepository.GetCategory(CategoryId));
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {CategoryId} {ModuleId}", CategoryId, ModuleId);
            return null;
        }
    }

    public Task<Models.Category> AddCategoryAsync(Models.Category Category)
    {
        if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, Category.ModuleId, PermissionNames.Edit))
        {
            Category = _CategoryRepository.AddCategory(Category);
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "Category Added {Category}", Category);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Add Attempt {Category}", Category);
            Category = null;
        }
        return Task.FromResult(Category);
    }

    public Task<Models.Category> UpdateCategoryAsync(Models.Category Category)
    {
        if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, Category.ModuleId, PermissionNames.Edit))
        {
            Category = _CategoryRepository.UpdateCategory(Category);
            _logger.Log(LogLevel.Information, this, LogFunction.Update, "Category Updated {Category}", Category);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Update Attempt {Category}", Category);
            Category = null;
        }
        return Task.FromResult(Category);
    }

    public Task DeleteCategoryAsync(int CategoryId, int ModuleId)
    {
        if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
        {
            _CategoryRepository.DeleteCategory(CategoryId);
            _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Category Deleted {CategoryId}", CategoryId);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Delete Attempt {CategoryId} {ModuleId}", CategoryId, ModuleId);
        }
        return Task.CompletedTask;
    }
}
