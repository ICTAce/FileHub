// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record CreateFileRequest : RequestBase, IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileSize { get; set; } = string.Empty;
    public int Downloads { get; set; }
}

public class CreateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<CreateFileRequest, int>
{
    private static readonly CreateMapper _mapper = new();

    public Task<int> Handle(CreateFileRequest request, CancellationToken cancellationToken)
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
    internal partial ICTAce.FileHub.Persistence.Entities.File ToEntity(CreateFileRequest request);
}
