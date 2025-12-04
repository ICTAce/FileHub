// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record GetCategoryRequest : EntityRequestBase, IRequest<GetCategoryDto>;

public class GetHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetCategoryRequest, GetCategoryDto?>
{
    private static readonly GetMapper _mapper = new();

    public Task<GetCategoryDto?> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
    {
        return HandleGetAsync<GetCategoryRequest, Persistence.Entities.Category, GetCategoryDto>(
            request: request,
            mapToResponse: _mapper.ToGetResponse,
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    [MapProperty(nameof(Persistence.Entities.Category.ParentId), nameof(GetCategoryDto.ParentId), Use = nameof(ConvertParentId))]
    public partial GetCategoryDto ToGetResponse(Persistence.Entities.Category category);
    
    private int ConvertParentId(int? parentId) => parentId ?? 0;
}
