// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Files;

public record GetFileRequest : EntityRequestBase, IRequest<GetFileDto?>;

public record GetFileDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required string ImageName { get; set; }
    public string? Description { get; set; }
    public required string FileSize { get; set; }
    public int Downloads { get; set; }
    public List<int> CategoryIds { get; set; } = [];

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}

public class GetHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetFileRequest, GetFileDto?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetFileDto?> Handle(GetFileRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized File Get Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return null;
        }

        using var db = CreateDbContext();
        var entity = await db.Set<Persistence.Entities.File>()
            .Include(f => f.FileCategories)
            .SingleOrDefaultAsync(e => e.Id == request.Id && e.ModuleId == request.ModuleId, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Read, "File not found {Id} {ModuleId}", request.Id, request.ModuleId);
            return null;
        }

        var dto = _mapper.ToDto(entity);
        dto.CategoryIds = entity.FileCategories.Select(fc => fc.CategoryId).ToList();
        
        return dto;
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    internal partial GetFileDto ToDto(Persistence.Entities.File entity);
}
