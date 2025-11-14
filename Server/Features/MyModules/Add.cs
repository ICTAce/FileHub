namespace ICTAce.FileHub.Features.MyModules;

// Command
public class AddMyModuleCommand : IRequest<Models.MyModule>
{
    public Models.MyModule MyModule { get; set; }
}

// Handler
public class AddHandler : IRequestHandler<AddMyModuleCommand, Models.MyModule>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IUserPermissions _userPermissions;
    private readonly ITenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogManager _logger;

    public AddHandler(
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

    public async Task<Models.MyModule> Handle(AddMyModuleCommand request, CancellationToken cancellationToken)
    {
        var alias = _tenantManager.GetAlias();
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user != null && _userPermissions.IsAuthorized(user, alias.SiteId, EntityNames.Module, request.MyModule.ModuleId, PermissionNames.Edit))
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            db.MyModule.Add(request.MyModule);
            await db.SaveChangesAsync(cancellationToken);
            
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "MyModule Added {MyModule}", request.MyModule);
            return request.MyModule;
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Add Attempt {MyModule}", request.MyModule);
            return null;
        }
    }
}
