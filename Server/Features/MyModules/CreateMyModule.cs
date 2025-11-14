namespace ICTAce.FileHub.Features.MyModules;

/// <summary>
/// Creates a new MyModule entry
/// </summary>
/// <remarks>
/// Business Rules:
/// - User must have Edit permission
/// - Name is required (1-100 characters)
/// - ModuleId must exist
/// 
/// Returns: MyModuleId of created entry
/// </remarks>
// ========== REQUEST ==========
public class CreateMyModule : IRequest<Result<int>>
{
    public int ModuleId { get; set; }
    public string Name { get; set; }
}

// ========== HANDLER ==========
public class CreateMyModuleHandler : IRequestHandler<CreateMyModule, Result<int>>
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IUserPermissions _userPermissions;
    private readonly ITenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateMyModuleHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _contextFactory = contextFactory;
        _userPermissions = userPermissions;
        _tenantManager = tenantManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<int>> Handle(CreateMyModule request, CancellationToken cancellationToken)
    {
        // Get current context
        var alias = _tenantManager.GetAlias();
        var user = _httpContextAccessor.HttpContext?.User;

        // Authorization check
        if (user == null || !_userPermissions.IsAuthorized(user, alias.SiteId, EntityNames.Module, request.ModuleId, PermissionNames.Edit))
        {
            return Result<int>.Unauthorized($"User not authorized to create MyModule in module {request.ModuleId}");
        }

        // Business logic
        var myModule = new Entities.MyModule
        {
            ModuleId = request.ModuleId,
            Name = request.Name
        };

        // Data access
        using var db = _contextFactory.CreateDbContext();
        db.MyModule.Add(myModule);
        await db.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(myModule.MyModuleId);
    }
}
