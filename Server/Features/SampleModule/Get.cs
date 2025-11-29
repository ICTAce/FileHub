// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record GetSampleModuleRequest : EntityRequestBase, IRequest<GetSampleModuleDto>;

public class GetHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetSampleModuleRequest, GetSampleModuleDto?>
{
    private static readonly GetMapper _mapper = new();

    public Task<GetSampleModuleDto?> Handle(GetSampleModuleRequest request, CancellationToken cancellationToken)
    {
        return HandleGetAsync<GetSampleModuleRequest, Persistence.Entities.SampleModule, GetSampleModuleDto>(
            request: request,
            mapToResponse: _mapper.ToGetResponse,
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    /// <summary>
    /// Maps SampleModule entity to GetSampleModuleResponse DTO
    /// </summary>
    public partial GetSampleModuleDto ToGetResponse(Persistence.Entities.SampleModule sampleModule);
}
