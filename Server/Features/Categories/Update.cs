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
    public Task<int> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        // Convert ParentId of 0 to null for root-level categories
        int? parentId = request.ParentId == 0 ? null : request.ParentId;

        return HandleUpdateAsync<UpdateCategoryRequest, Persistence.Entities.Category>(
            request: request,
            setPropertyCalls: setter => setter
                .SetProperty(e => e.Name, request.Name)
                .SetProperty(e => e.ViewOrder, request.ViewOrder)
                .SetProperty(e => e.ParentId, parentId),
            cancellationToken: cancellationToken
        );
    }
}
