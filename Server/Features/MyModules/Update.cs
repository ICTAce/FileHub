namespace ICTAce.FileHub.Features.MyModules;

// Command
public class UpdateMyModuleCommand : IRequest<Models.MyModule>
{
    public Models.MyModule MyModule { get; set; }
}

// Handler
public class UpdateHandler : IRequestHandler<UpdateMyModuleCommand, Models.MyModule>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IUserPermissions _userPermissions;
    private readonly ITenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogManager _logger;

    public UpdateHandler(
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

    public async Task<Models.MyModule> Handle(UpdateMyModuleCommand request, CancellationToken cancellationToken)
    {
        var alias = _tenantManager.GetAlias();
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user != null && _userPermissions.IsAuthorized(user, alias.SiteId, EntityNames.Module, request.MyModule.ModuleId, PermissionNames.Edit))
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            db.Entry(request.MyModule).State = EntityState.Modified;
            await db.SaveChangesAsync(cancellationToken);
            
            _logger.Log(LogLevel.Information, this, LogFunction.Update, "MyModule Updated {MyModule}", request.MyModule);
            return request.MyModule;
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Update Attempt {MyModule}", request.MyModule);
            return null;
        }
    }
}
