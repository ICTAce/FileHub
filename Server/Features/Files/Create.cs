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
    public List<int> CategoryIds { get; set; } = [];
}

public class CreateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<CreateFileRequest, int>
{
    private static readonly CreateMapper _mapper = new();

    public async Task<int> Handle(CreateFileRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized File Add Attempt {ModuleId}", request.ModuleId);
            return -1;
        }

        var entity = _mapper.ToEntity(request);

        using var db = CreateDbContext();
        db.Set<Persistence.Entities.File>().Add(entity);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Save file-category relationships
        if (request.CategoryIds.Any())
        {
            foreach (var categoryId in request.CategoryIds)
            {
                var fileCategory = new Persistence.Entities.FileCategory
                {
                    FileId = entity.Id,
                    CategoryId = categoryId,
                    ModuleId = entity.ModuleId,
                    CreatedBy = entity.CreatedBy,
                    CreatedOn = entity.CreatedOn
                };
                db.Set<Persistence.Entities.FileCategory>().Add(fileCategory);
            }
            var result = await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        Logger.Log(LogLevel.Information, this, LogFunction.Create, "File Added {Entity}", entity);
        return entity.Id;
    }
}

[Mapper]
internal sealed partial class CreateMapper
{
    internal partial ICTAce.FileHub.Persistence.Entities.File ToEntity(CreateFileRequest request);
}
