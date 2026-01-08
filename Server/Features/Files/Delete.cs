// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record DeleteFileRequest : EntityRequestBase, IRequest<int>;

public class DeleteHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<DeleteFileRequest, int>
{
    public Task<int> Handle(DeleteFileRequest request, CancellationToken cancellationToken)
    {
        return HandleDeleteAsync<DeleteFileRequest, Persistence.Entities.File>(
            request: request,
            cancellationToken: cancellationToken
        );
    }
}
