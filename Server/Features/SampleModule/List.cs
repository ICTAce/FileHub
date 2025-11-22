// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record ListSampleModuleRequest : RequestBase, IRequest<PagedResult<ListSampleModuleDto>>
{
    /// <summary>
    /// Page number (1-based). Defaults to 1 if not specified.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10 if not specified.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
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

            // Get total count for pagination metadata
            var totalCount = await db.SampleModule
                .Where(item => item.ModuleId == request.ModuleId)
                .CountAsync(cancellationToken).ConfigureAwait(false);

            // Apply pagination
            var modules = await db.SampleModule
                .Where(item => item.ModuleId == request.ModuleId)
                .OrderBy(m => m.Name) // Consistent ordering for pagination
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            // Map MyModule entities to ListMyModuleResponse DTOs using Mapperly
            var items = modules
                .Select(_mapper.ToListResponse)
                .ToList();

            return new PagedResult<ListSampleModuleDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {ModuleId}", request.ModuleId);
            return null;
        }
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    /// <summary>
    /// Maps MyModule entity to ListMyModuleResponse DTO
    /// </summary>
    public partial ListSampleModuleDto ToListResponse(Persistence.Entities.SampleModule myModule);
}
