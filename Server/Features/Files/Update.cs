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
    public List<int> CategoryIds { get; set; } = [];
}

public class UpdateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<UpdateFileRequest, int>
{
    public async Task<int> Handle(UpdateFileRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized File Update Attempt {Id}", request.Id);
            return -1;
        }

        using var db = CreateDbContext();

        // Update file properties
        var rowsAffected = await db.Set<Persistence.Entities.File>()
            .Where(e => e.Id == request.Id && e.ModuleId == request.ModuleId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(e => e.Name, request.Name)
                .SetProperty(e => e.FileName, request.FileName)
                .SetProperty(e => e.ImageName, request.ImageName)
                .SetProperty(e => e.Description, request.Description)
                .SetProperty(e => e.FileSize, request.FileSize)
                .SetProperty(e => e.Downloads, request.Downloads),
                cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected > 0)
        {
            // Update file-category relationships
            // First, remove existing relationships
            await db.Set<Persistence.Entities.FileCategory>()
                .Where(fc => fc.FileId == request.Id)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);

            // Then, add new relationships
            if (request.CategoryIds.Any())
            {
                foreach (var categoryId in request.CategoryIds)
                {
                    var fileCategory = new Persistence.Entities.FileCategory
                    {
                        FileId = request.Id,
                        CategoryId = categoryId
                    };
                    db.Set<Persistence.Entities.FileCategory>().Add(fileCategory);
                }
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            Logger.Log(LogLevel.Information, this, LogFunction.Update, "File Updated {Id}", request.Id);
            return request.Id;
        }

        Logger.Log(LogLevel.Warning, this, LogFunction.Update, "File Not Found {Id}", request.Id);
        return -1;
    }
}
