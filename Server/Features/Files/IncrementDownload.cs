// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record IncrementDownloadRequest : IRequest<int>
{
    public int FileId { get; set; }
    public int ModuleId { get; set; }
}

public class IncrementDownloadHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<IncrementDownloadRequest, int>
{
    public async Task<int> Handle(IncrementDownloadRequest request, CancellationToken cancellationToken)
    {
        using var db = CreateDbContext();

        var rowsAffected = await db.File
            .Where(f => f.Id == request.FileId && f.ModuleId == request.ModuleId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(f => f.Downloads, f => f.Downloads + 1),
                cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected > 0)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Update,
                "Download counter incremented FileId={FileId} ModuleId={ModuleId}", 
                request.FileId, request.ModuleId);
            return request.FileId;
        }

        Logger.Log(LogLevel.Warning, this, LogFunction.Update,
            "Failed to increment download counter FileId={FileId} ModuleId={ModuleId}", 
            request.FileId, request.ModuleId);
        return -1;
    }
}
