// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record DeleteCategoryRequest : EntityRequestBase, IRequest<int>;

public class DeleteHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<DeleteCategoryRequest, int>
{
    public Task<int> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        return HandleDeleteAsync<DeleteCategoryRequest, Persistence.Entities.Category>(
            request: request,
            cancellationToken: cancellationToken
        );
    }
}
