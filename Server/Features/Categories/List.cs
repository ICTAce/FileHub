// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record ListCategoryRequest : RequestBase, IRequest<PagedResult<ListCategoryDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class ListHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<ListCategoryRequest, PagedResult<ListCategoryDto>?>
{
    private static readonly ListMapper _mapper = new();

    public async Task<PagedResult<ListCategoryDto>?> Handle(ListCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();

            var totalCount = await db.Category
                .Where(item => item.ModuleId == request.ModuleId)
                .CountAsync(cancellationToken).ConfigureAwait(false);

            var categories = await db.Category
                .Where(item => item.ModuleId == request.ModuleId)
                .OrderBy(c => c.ViewOrder)
                .ThenBy(c => c.Name) // Consistent ordering for pagination
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var items = categories
                .Select(_mapper.ToListResponse)
                .ToList();

            return new PagedResult<ListCategoryDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
            };
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized FileHUb Category Get Attempt {ModuleId}", request.ModuleId);
        return null;
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    public partial ListCategoryDto ToListResponse(Persistence.Entities.Category category);
}
