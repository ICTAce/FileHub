using System.Security.Claims;
using MediatR;

namespace ICTAce.FileHub.Features.MyModules;

// Command
public class DeleteMyModuleCommand : IRequest<Unit>
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
}

// Handler
public class DeleteMyModuleHandler : IRequestHandler<DeleteMyModuleCommand, Unit>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IUserPermissions _userPermissions;
    private readonly ITenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogManager _logger;

    public DeleteMyModuleHandler(
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

    public async Task<Unit> Handle(DeleteMyModuleCommand request, CancellationToken cancellationToken)
    {
        var alias = _tenantManager.GetAlias();
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user != null && _userPermissions.IsAuthorized(user, alias.SiteId, EntityNames.Module, request.ModuleId, PermissionNames.Edit))
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            var myModule = await db.MyModule.FindAsync([request.MyModuleId], cancellationToken);
            if (myModule != null)
            {
                db.MyModule.Remove(myModule);
                await db.SaveChangesAsync(cancellationToken);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "MyModule Deleted {MyModuleId}", request.MyModuleId);
            }
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {MyModuleId} {ModuleId}", request.MyModuleId, request.ModuleId);
        }
        
        return Unit.Value;
    }
}
