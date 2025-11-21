// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Categories;

public class ListHandler(
    IDbContextFactory<ApplicationQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<ListCategoryRequest, PagedResult<ListCategoryResponse>?>
{
    private static readonly ListMapper _mapper = new();

    public async Task<PagedResult<ListCategoryResponse>?> Handle(ListCategoryRequest request, CancellationToken cancellationToken)
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

            return new PagedResult<ListCategoryResponse>
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
    public partial ListCategoryResponse ToListResponse(Persistence.Entities.Category category);
}
