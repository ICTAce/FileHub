using System.Security.Claims;
using MediatR;

namespace ICTAce.FileHub.Features.MyModules;

// Query
public class GetMyModuleByIdQuery : IRequest<Models.MyModule>
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
}

// Handler
public class GetMyModuleByIdHandler : IRequestHandler<GetMyModuleByIdQuery, Models.MyModule>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IUserPermissions _userPermissions;
    private readonly ITenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogManager _logger;

    public GetMyModuleByIdHandler(
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

    public async Task<Models.MyModule> Handle(GetMyModuleByIdQuery request, CancellationToken cancellationToken)
    {
        var alias = _tenantManager.GetAlias();
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user != null && _userPermissions.IsAuthorized(user, alias.SiteId, EntityNames.Module, request.ModuleId, PermissionNames.View))
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            return await db.MyModule.FindAsync([request.MyModuleId], cancellationToken);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {MyModuleId} {ModuleId}", request.MyModuleId, request.ModuleId);
            return null;
        }
    }
}
