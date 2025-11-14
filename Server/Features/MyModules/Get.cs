using ICTAce.FileHub.Client.Features.MyModules;

namespace ICTAce.FileHub.Features.MyModules;

// Handler
public class GetHandler : CommandHandlerBase, IRequestHandler<GetMyModuleRequest, GetMyModuleResponse>
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

    public async Task<GetMyModuleResponse> Handle(GetMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var entity = await db.MyModule.FindAsync(new object[] { request.MyModuleId }, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "MyModule not found {MyModuleId} {ModuleId}", request.MyModuleId, request.ModuleId);
                return null;
            }

            return new GetMyModuleResponse
            {
                MyModuleId = entity.MyModuleId,
                ModuleId = entity.ModuleId,
                Name = entity.Name,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
                ModifiedBy = entity.ModifiedBy,
                ModifiedOn = entity.ModifiedOn
            };
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {MyModuleId} {ModuleId}", request.MyModuleId, request.ModuleId);
            return null;
        }
    }
}
