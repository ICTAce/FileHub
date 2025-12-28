// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record GetFileByFileNameRequest : IRequest<FileModuleInfo?>
{
    public required string FileName { get; set; }
}

public record FileModuleInfo
{
    public int ModuleId { get; set; }
}

public class GetByFileNameHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetFileByFileNameRequest, FileModuleInfo?>
{
    public async Task<FileModuleInfo?> Handle(GetFileByFileNameRequest request, CancellationToken cancellationToken)
    {
        using var db = CreateDbContext();
        
        // Look up file by FileName or ImageName
        var file = await db.File
            .Where(f => f.FileName == request.FileName || f.ImageName == request.FileName)
            .Select(f => new FileModuleInfo { ModuleId = f.ModuleId })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return file;
    }
}
