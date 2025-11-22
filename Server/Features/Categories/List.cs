// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Client.Services.Common;

namespace ICTAce.FileHub.Features.Categories;

public record ListCategoryRequest : RequestBase, IRequest<PagedResult<ListCategoryDto>>
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
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<ListCategoryRequest, PagedResult<ListCategoryDto>?>
{
    private static readonly ListMapper _mapper = new();

    public async Task<PagedResult<ListCategoryDto>?> Handle(ListCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();

            // Get total count for pagination metadata
            var totalCount = await db.Category
                .Where(item => item.ModuleId == request.ModuleId)
                .CountAsync(cancellationToken).ConfigureAwait(false);

            // Apply pagination
            var categories = await db.Category
                .Where(item => item.ModuleId == request.ModuleId)
                .OrderBy(c => c.ViewOrder)
                .ThenBy(c => c.Name) // Consistent ordering for pagination
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            // Map Category entities to ListCategoryResponse DTOs using Mapperly
            var items = categories
                .Select(_mapper.ToListResponse)
                .ToList();

            return new PagedResult<ListCategoryDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {ModuleId}", request.ModuleId);
            return null;
        }
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    /// <summary>
    /// Maps Category entity to ListCategoryResponse DTO
    /// </summary>
    public partial ListCategoryDto ToListResponse(Persistence.Entities.Category category);
}
