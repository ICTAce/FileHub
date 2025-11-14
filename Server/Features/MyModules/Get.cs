namespace ICTAce.FileHub.Features.MyModules;

// Query
public class GetMyModuleByIdQuery : IRequest<Models.MyModule>
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
}

// Handler
public class GetHandler : CommandHandlerBase, IRequestHandler<GetMyModuleByIdQuery, Models.MyModule>
{
    public GetHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<Models.MyModule> Handle(GetMyModuleByIdQuery request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            return await db.MyModule.FindAsync([request.MyModuleId], cancellationToken);
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {MyModuleId} {ModuleId}", request.MyModuleId, request.ModuleId);
            return null;
        }
    }
}
