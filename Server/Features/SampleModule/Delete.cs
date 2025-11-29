// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record DeleteSampleModuleRequest : EntityRequestBase, IRequest<int>;

public class DeleteHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<DeleteSampleModuleRequest, int>
{
    public Task<int> Handle(DeleteSampleModuleRequest request, CancellationToken cancellationToken)
    {
        return HandleDeleteAsync<DeleteSampleModuleRequest, Persistence.Entities.SampleModule>(
            request: request,
            cancellationToken: cancellationToken
        );
    }
}
