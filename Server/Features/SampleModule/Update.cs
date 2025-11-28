// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record UpdateSampleModuleRequest : EntityRequestBase, IRequest<int>
{
    public required string Name { get; set; }
}

public class UpdateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<UpdateSampleModuleRequest, int>
{
    private static readonly UpdateMapper _mapper = new();

    public Task<int> Handle(UpdateSampleModuleRequest request, CancellationToken cancellationToken)
    {
        return HandleUpdateAsync<UpdateSampleModuleRequest, Persistence.Entities.SampleModule>(
            request: request,
            updateEntity: _mapper.ApplyUpdate,
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class UpdateMapper
{
    internal partial void ApplyUpdate(Persistence.Entities.SampleModule entity, UpdateSampleModuleRequest request);
}
