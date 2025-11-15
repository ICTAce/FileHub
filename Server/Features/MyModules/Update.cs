// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

// Handler
public class UpdateHandler(
    IDbContextFactory<Context> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<UpdateMyModuleRequest, int>
{
    public async Task<int> Handle(UpdateMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();

            // Fetch existing entity
            var myModule = await db.MyModule.FindAsync(new object[] { request.Id }, cancellationToken);
            if (myModule != null)
            {
                // Update only user-editable fields
                myModule.Name = request.Name;
                // ModifiedBy, ModifiedOn will be updated by IAuditable/database

                await db.SaveChangesAsync(cancellationToken);

                Logger.Log(LogLevel.Information, this, LogFunction.Update, "MyModule Updated {MyModule}", myModule);
                return request.Id;
            }

            return -1;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Update Attempt {Id}", request.Id);
            return -1;
        }
    }
}
