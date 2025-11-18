// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.MyModules;

public class GetHandler(
    IDbContextFactory<MyModuleCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<GetMyModuleRequest, GetMyModuleResponse?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetMyModuleResponse?> Handle(GetMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var entity = await db.MyModule.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "MyModule not found {Id} {ModuleId}", request.Id, request.ModuleId);
                return null;
            }

            return _mapper.ToGetResponse(entity);
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return null;
        }
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    /// <summary>
    /// Maps MyModule entity to GetMyModuleResponse DTO
    /// </summary>
    public partial GetMyModuleResponse ToGetResponse(Persistence.Entities.MyModule myModule);
}
