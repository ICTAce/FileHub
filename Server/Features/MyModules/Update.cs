using ICTAce.FileHub.Client.Features.MyModules;

namespace ICTAce.FileHub.Features.MyModules;

// Wrapper that implements IRequest for MediatR
public class UpdateMyModuleRequest : UpdateMyModuleCommand, IRequest<Models.MyModule>
{
}

// Handler
public class UpdateHandler : CommandHandlerBase, IRequestHandler<UpdateMyModuleRequest, Models.MyModule>
{
    public UpdateHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<Models.MyModule> Handle(UpdateMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();
            
            // Fetch existing entity
            var myModule = await db.MyModule.FindAsync(new object[] { request.MyModuleId }, cancellationToken);
            if (myModule != null)
            {
                // Update only user-editable fields
                myModule.Name = request.Name;
                // ModifiedBy, ModifiedOn will be updated by IAuditable/database
                
                await db.SaveChangesAsync(cancellationToken);
                
                Logger.Log(LogLevel.Information, this, LogFunction.Update, "MyModule Updated {MyModule}", myModule);
                return myModule;
            }
            
            return null;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Update Attempt {MyModuleId}", request.MyModuleId);
            return null;
        }
    }
}
