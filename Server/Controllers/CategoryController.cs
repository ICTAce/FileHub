namespace ICTAce.FileHub.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class CategoryController : ModuleControllerBase
{
    private readonly ICategoryService _CategoryService;

    public CategoryController(ICategoryService CategoryService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
    {
        _CategoryService = CategoryService;
    }

    // GET: api/<controller>?moduleid=x
    [HttpGet]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<IEnumerable<Models.Category>> Get(string moduleid)
    {
        int ModuleId;
        if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
        {
            return await _CategoryService.GetCategoriesAsync(ModuleId);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {ModuleId}", moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // GET: api/<controller>/parent?parentid=x&moduleid=y
    [HttpGet("parent")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<IEnumerable<Models.Category>> GetByParent(string parentid, string moduleid)
    {
        int ModuleId;
        int? ParentId = null;
        
        if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
        {
            if (!string.IsNullOrEmpty(parentid) && int.TryParse(parentid, out int parsedParentId))
            {
                ParentId = parsedParentId;
            }
            
            return await _CategoryService.GetCategoriesByParentAsync(ParentId, ModuleId);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {ModuleId}", moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // GET api/<controller>/5
    [HttpGet("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.ViewModule)]
    public async Task<Models.Category> Get(int id, int moduleid)
    {
        Models.Category Category = await _CategoryService.GetCategoryAsync(id, moduleid);
        if (Category != null && IsAuthorizedEntityId(EntityNames.Module, Category.ModuleId))
        {
            return Category;
        }
        else
        { 
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {CategoryId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }
    }

    // POST api/<controller>
    [HttpPost]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<Models.Category> Post([FromBody] Models.Category Category)
    {
        if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, Category.ModuleId))
        {
            Category = await _CategoryService.AddCategoryAsync(Category);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Post Attempt {Category}", Category);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Category = null;
        }
        return Category;
    }

    // PUT api/<controller>/5
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task<Models.Category> Put(int id, [FromBody] Models.Category Category)
    {
        if (ModelState.IsValid && Category.CategoryId == id && IsAuthorizedEntityId(EntityNames.Module, Category.ModuleId))
        {
            Category = await _CategoryService.UpdateCategoryAsync(Category);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Put Attempt {Category}", Category);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Category = null;
        }
        return Category;
    }

    // DELETE api/<controller>/5
    [HttpDelete("{id}/{moduleid}")]
    [Authorize(Policy = PolicyNames.EditModule)]
    public async Task Delete(int id, int moduleid)
    {
        Models.Category Category = await _CategoryService.GetCategoryAsync(id, moduleid);
        if (Category != null && IsAuthorizedEntityId(EntityNames.Module, Category.ModuleId))
        {
            await _CategoryService.DeleteCategoryAsync(id, Category.ModuleId);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Delete Attempt {CategoryId} {ModuleId}", id, moduleid);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}
