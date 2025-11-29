// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record CreateSampleModuleRequest : RequestBase, IRequest<int>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<CreateSampleModuleRequest, int>
{
    private static readonly CreateMapper _mapper = new();

    public Task<int> Handle(CreateSampleModuleRequest request, CancellationToken cancellationToken)
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
    internal partial Persistence.Entities.SampleModule ToEntity(CreateSampleModuleRequest request);
}
