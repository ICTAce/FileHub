// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

public record CreateMyModuleRequest : IRequest<int>
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateHandler(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<CreateMyModuleRequest, int>
{
    public async Task<int> Handle(CreateMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            // Build the entity from command data
            var myModule = new Persistence.Entities.MyModule
            {
                ModuleId = request.ModuleId,
                Name = request.Name
                // CreatedBy, CreatedOn, ModifiedBy, ModifiedOn will be set by IAuditable/database
            };

            using var db = CreateDbContext();
            db.MyModule.Add(myModule);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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
