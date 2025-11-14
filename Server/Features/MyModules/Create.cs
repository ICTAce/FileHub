// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

// Handler
public class CreateHandler : CommandHandlerBase, IRequestHandler<CreateMyModuleRequest, int>
{
    public CreateHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<int> Handle(CreateMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            // Build the entity from command data
            var myModule = new Entities.MyModule
            {
                ModuleId = request.ModuleId,
                Name = request.Name
                // CreatedBy, CreatedOn, ModifiedBy, ModifiedOn will be set by IAuditable/database
            };

            using var db = CreateDbContext();
            db.MyModule.Add(myModule);
            await db.SaveChangesAsync(cancellationToken);
            
            Logger.Log(LogLevel.Information, this, LogFunction.Create, "MyModule Added {MyModule}", myModule);
            return myModule.Id;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Add Attempt {ModuleId} {Name}", request.ModuleId, request.Name);
            return -1;
        }
    }
}
