// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record CreateCategoryRequest : RequestBase, IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }
}

public class CreateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<CreateCategoryRequest, int>
{
    private static readonly CreateMapper _mapper = new();

    public Task<int> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        return HandleCreateAsync(
            request: request,
            mapToEntity: _mapper.ToEntity,
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class CreateMapper
{
    [MapProperty(nameof(CreateCategoryRequest.ParentId), nameof(Persistence.Entities.Category.ParentId), Use = nameof(ConvertParentId))]
    internal partial Persistence.Entities.Category ToEntity(CreateCategoryRequest request);

    private int? ConvertParentId(int parentId) => parentId == 0 ? null : parentId;
}
