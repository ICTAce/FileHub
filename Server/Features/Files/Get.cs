// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record GetFileRequest : EntityRequestBase, IRequest<GetFileDto>;

public class GetHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetFileRequest, GetFileDto?>
{
    private static readonly GetMapper _mapper = new();

    public Task<GetFileDto?> Handle(GetFileRequest request, CancellationToken cancellationToken)
    {
        return HandleGetAsync<GetFileRequest, Persistence.Entities.File, GetFileDto>(
            request: request,
            mapToResponse: _mapper.ToGetResponse,
            cancellationToken: cancellationToken
        );
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    public partial GetFileDto ToGetResponse(ICTAce.FileHub.Persistence.Entities.File file);
}
