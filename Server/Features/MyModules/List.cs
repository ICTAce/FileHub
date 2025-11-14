namespace ICTAce.FileHub.Features.MyModules;

// Query
public class GetMyModulesQuery : IRequest<List<Models.MyModule>>
{
    public int ModuleId { get; set; }
}

// Handler
public class ListHandler : CommandHandlerBase, IRequestHandler<GetMyModulesQuery, List<Models.MyModule>>
{
    public ListHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<List<Models.MyModule>> Handle(GetMyModulesQuery request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            return await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .ToListAsync(cancellationToken);
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {ModuleId}", request.ModuleId);
            return null;
        }
    }
}
