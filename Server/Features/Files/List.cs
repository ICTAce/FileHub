// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record ListFileRequest : PagedRequestBase, IRequest<PagedResult<ListFileDto>>;

public class ListHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<ListFileRequest, PagedResult<ListFileDto>?>
{
    private static readonly ListMapper _mapper = new();

    public Task<PagedResult<ListFileDto>?> Handle(ListFileRequest request, CancellationToken cancellationToken)
    {
        return HandleListAsync<ListFileRequest, Persistence.Entities.File, ListFileDto>(
            request: request,
            mapToResponse: _mapper.ToListResponse,
            orderBy: query => query.OrderBy(f => f.Name),
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class ListMapper
{
    public partial ListFileDto ToListResponse(ICTAce.FileHub.Persistence.Entities.File file);
}
