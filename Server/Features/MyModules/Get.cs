// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.MyModules;

public record GetMyModuleRequest : RequestBase, IRequest<GetMyModuleDto>
{
    public int Id { get; set; }
}

public class GetHandler(
    IDbContextFactory<ApplicationQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<GetMyModuleRequest, GetMyModuleDto?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetMyModuleDto?> Handle(GetMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            // Use SingleOrDefaultAsync to ensure ModuleId scoping is applied at query level
            var entity = await db.MyModule.SingleOrDefaultAsync(m => m.Id == request.Id && m.ModuleId == request.ModuleId, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "MyModule not found Id={Id} in ModuleId= {ModuleId}", request.Id, request.ModuleId);
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
    public partial GetMyModuleDto ToGetResponse(Persistence.Entities.MyModule myModule);
}
