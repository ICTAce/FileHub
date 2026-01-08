// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record GetFileByFileNameRequest : IRequest<FileModuleInfo?>
{
    public required string FileName { get; set; }
}

public record FileModuleInfo
{
    public int FileId { get; set; }
    public int ModuleId { get; set; }
}

public class GetByFileNameHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetFileByFileNameRequest, FileModuleInfo?>
{
    public async Task<FileModuleInfo?> Handle(GetFileByFileNameRequest request, CancellationToken cancellationToken)
    {
        using var db = CreateDbContext();
        
        var trimmedFileName = request.FileName.Trim();
        
        // Look up file by FileName or ImageName (case-insensitive)
        var file = await db.File
            .Where(f => f.FileName == trimmedFileName || f.ImageName == trimmedFileName)
            .Select(f => new FileModuleInfo { FileId = f.Id, ModuleId = f.ModuleId })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        // Log if not found for debugging
        if (file is null)
        {
            Logger.Log(LogLevel.Warning, this, LogFunction.Read,
                "GetByFileName: File not found in database FileName={FileName}", trimmedFileName);
        }
        else
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Read,
                "GetByFileName: File found FileId={FileId} ModuleId={ModuleId} FileName={FileName}", 
                file.FileId, file.ModuleId, trimmedFileName);
        }

        return file;
    }
}
