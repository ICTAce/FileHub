// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record UpdateSampleModuleRequest : RequestBase, IRequest<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class UpdateHandler(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<UpdateSampleModuleRequest, int>
{
    public async Task<int> Handle(UpdateSampleModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();

            // Fetch existing entity
            var myModule = await db.SampleModule.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            if (myModule != null)
            {
                myModule.Name = request.Name;

                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                Logger.Log(LogLevel.Information, this, LogFunction.Update, "SampleModule Updated {SampleModule}", myModule);
                return request.Id;
            }

            return -1;
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized SampleModule Update Attempt {Id}", request.Id);
        return -1;
    }
}
