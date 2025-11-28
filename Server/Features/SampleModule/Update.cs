// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record UpdateSampleModuleRequest : EntityRequestBase, IRequest<int>
{
    public required string Name { get; set; }
}

public class UpdateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<UpdateSampleModuleRequest, int>
{
    public Task<int> Handle(UpdateSampleModuleRequest request, CancellationToken cancellationToken)
    {
        return HandleUpdateAsync<UpdateSampleModuleRequest, Persistence.Entities.SampleModule>(
            request: request,
            setPropertyCalls: setter => setter.SetProperty(e => e.Name, request.Name),
            cancellationToken: cancellationToken
        );
    }
}
