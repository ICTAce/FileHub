// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.SampleModule;

public record ListSampleModuleRequest : PagedRequestBase, IRequest<PagedResult<ListSampleModuleDto>>;

public class ListHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<ListSampleModuleRequest, PagedResult<ListSampleModuleDto>?>
{
    private static readonly ListMapper _mapper = new();

    public Task<PagedResult<ListSampleModuleDto>?> Handle(ListSampleModuleRequest request, CancellationToken cancellationToken)
    {
        return HandleListAsync<ListSampleModuleRequest, Persistence.Entities.SampleModule, ListSampleModuleDto>(
            request: request,
            mapToResponse: _mapper.ToListResponse,
            orderBy: query => query.OrderBy(m => m.Name),
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    public partial ListSampleModuleDto ToListResponse(Persistence.Entities.SampleModule sampleModule);
}
