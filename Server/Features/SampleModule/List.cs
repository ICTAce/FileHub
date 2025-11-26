// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record ListSampleModuleRequest : RequestBase, IRequest<PagedResult<ListSampleModuleDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ListHandler(
    IDbContextFactory<ApplicationQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<ListSampleModuleRequest, PagedResult<ListSampleModuleDto>?>
{
    private static readonly ListMapper _mapper = new();

    public async Task<PagedResult<ListSampleModuleDto>?> Handle(ListSampleModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();

            var totalCount = await db.SampleModule
                .Where(item => item.ModuleId == request.ModuleId)
                .CountAsync(cancellationToken).ConfigureAwait(false);

            var sampleModules = await db.SampleModule
                .Where(item => item.ModuleId == request.ModuleId)
                .OrderBy(m => m.Name) // Consistent ordering for pagination
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var items = sampleModules
                .Select(_mapper.ToListResponse)
                .ToList();

            return new PagedResult<ListSampleModuleDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
            };
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized SampleModule Get Attempt {ModuleId}", request.ModuleId);
        return null;
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    public partial ListSampleModuleDto ToListResponse(Persistence.Entities.SampleModule sampleModule);
}
