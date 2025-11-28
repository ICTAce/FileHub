// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record UpdateCategoryRequest : EntityRequestBase, IRequest<int>
{
    public required string Name { get; set; }
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }
}

public class UpdateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<UpdateCategoryRequest, int>
{
    private static readonly UpdateMapper _mapper = new();

    public Task<int> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        return HandleUpdateAsync<UpdateCategoryRequest, Persistence.Entities.Category>(
            request: request,
            updateEntity: _mapper.ApplyUpdate,
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class UpdateMapper
{
    internal partial void ApplyUpdate(Persistence.Entities.Category entity, UpdateCategoryRequest request);
}
