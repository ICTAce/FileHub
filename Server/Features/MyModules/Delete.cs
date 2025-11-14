// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.MyModules;

// Handler
public class DeleteHandler : CommandHandlerBase, IRequestHandler<DeleteMyModuleRequest, int>
{
    public DeleteHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<int> Handle(DeleteMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();
            var myModule = await db.MyModule.FindAsync(new object[] { request.Id }, cancellationToken);
            if (myModule != null)
            {
                db.MyModule.Remove(myModule);
                await db.SaveChangesAsync(cancellationToken);
                Logger.Log(LogLevel.Information, this, LogFunction.Delete, "MyModule Deleted {Id}", request.Id);
                return request.Id;
            }
            return -1;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return -1;
        }
    }
}
