// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record GetMyModuleRequest : RequestBase, IRequest<GetSampleModuleDto>
{
    public int Id { get; set; }
}

public class GetHandler(
    IDbContextFactory<ApplicationQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<GetMyModuleRequest, GetSampleModuleDto?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetSampleModuleDto?> Handle(GetMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var entity = await db.SampleModule.SingleOrDefaultAsync(m => m.Id == request.Id && m.ModuleId == request.ModuleId, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "SampleModule not found Id={Id} in ModuleId= {ModuleId}", request.Id, request.ModuleId);
                return null;
            }

            return _mapper.ToGetResponse(entity);
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized SampleModule Get Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
        return null;
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    /// <summary>
    /// Maps MyModule entity to GetMyModuleResponse DTO
    /// </summary>
    public partial GetSampleModuleDto ToGetResponse(Persistence.Entities.SampleModule myModule);
}
