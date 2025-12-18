// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record UpdateFileRequest : EntityRequestBase, IRequest<int>
{
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required string ImageName { get; set; }
    public string? Description { get; set; }
    public required string FileSize { get; set; }
    public int Downloads { get; set; }
}

public class UpdateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<UpdateFileRequest, int>
{
    public Task<int> Handle(UpdateFileRequest request, CancellationToken cancellationToken)
    {
        return HandleUpdateAsync<UpdateFileRequest, Persistence.Entities.File>(
            request: request,
            setPropertyCalls: setter => setter
                .SetProperty(e => e.Name, request.Name)
                .SetProperty(e => e.FileName, request.FileName)
                .SetProperty(e => e.ImageName, request.ImageName)
                .SetProperty(e => e.Description, request.Description)
                .SetProperty(e => e.FileSize, request.FileSize)
                .SetProperty(e => e.Downloads, request.Downloads),
            cancellationToken: cancellationToken
        );
    }
}
