namespace ICTAce.FileHub.Features.MyModules;

// Query
public class GetMyModulesQuery : IRequest<List<Models.MyModule>>
{
    public int ModuleId { get; set; }
}

// Handler
public class GetMyModulesHandler : IRequestHandler<GetMyModulesQuery, List<Models.MyModule>>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IUserPermissions _userPermissions;
    private readonly ITenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogManager _logger;

    public GetMyModulesHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
    {
        _contextFactory = contextFactory;
        _userPermissions = userPermissions;
        _tenantManager = tenantManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<List<Models.MyModule>> Handle(GetMyModulesQuery request, CancellationToken cancellationToken)
    {
        var alias = _tenantManager.GetAlias();
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user != null && _userPermissions.IsAuthorized(user, alias.SiteId, EntityNames.Module, request.ModuleId, PermissionNames.View))
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            return await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .ToListAsync(cancellationToken);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {ModuleId}", request.ModuleId);
            return null;
        }
    }
}
