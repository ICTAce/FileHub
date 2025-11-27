// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record CreateSampleModuleRequest : IRequest<int>
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
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<CreateSampleModuleRequest, int>
{
    public async Task<int> Handle(CreateSampleModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            // Build the entity from command data
            var sampleModule = new Persistence.Entities.SampleModule
            {
                ModuleId = request.ModuleId,
                Name = request.Name,
                // CreatedBy, CreatedOn, ModifiedBy, ModifiedOn will be set by IAuditable/database
            };

            using var db = CreateDbContext();
            db.SampleModule.Add(sampleModule);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.Log(LogLevel.Information, this, LogFunction.Create, "SampleModule Added {SampleModule}", sampleModule);
            return sampleModule.Id;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized SampleModule Add Attempt {ModuleId} {Name}", request.ModuleId, request.Name);
            return -1;
        }
    }
}
