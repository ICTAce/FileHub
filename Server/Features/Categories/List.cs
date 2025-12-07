// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record ListCategoryRequest : PagedRequestBase, IRequest<PagedResult<ListCategoryDto>>;

public class ListHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<ListCategoryRequest, PagedResult<ListCategoryDto>?>
{
    private static readonly ListMapper _mapper = new();

    public Task<PagedResult<ListCategoryDto>?> Handle(ListCategoryRequest request, CancellationToken cancellationToken)
    {
        return HandleListAsync<ListCategoryRequest, Persistence.Entities.Category, ListCategoryDto>(
            request: request,
            mapToResponse: _mapper.ToListResponse,
            orderBy: query => query.OrderBy(c => c.ViewOrder).ThenBy(c => c.Name),
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    [MapProperty(nameof(Persistence.Entities.Category.ParentId), nameof(ListCategoryDto.ParentId), Use = nameof(ConvertParentId))]
    public partial ListCategoryDto ToListResponse(Persistence.Entities.Category category);

    private int ConvertParentId(int? parentId) => parentId ?? 0;
}
