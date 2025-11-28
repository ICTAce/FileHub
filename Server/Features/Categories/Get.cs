// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record GetCategoryRequest : RequestBase, IRequest<GetCategoryDto>
{
    public int Id { get; set; }
}

public class GetHandler(HandlerServices<ApplicationQueryContext> services)
    : HandlerBase<ApplicationQueryContext>(services), IRequestHandler<GetCategoryRequest, GetCategoryDto?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetCategoryDto?> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var entity = await db.Category.SingleOrDefaultAsync(m => m.Id == request.Id && m.ModuleId == request.ModuleId, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "FileHub Category not found {Id} {ModuleId}", request.Id, request.ModuleId);
                return null;
            }

            return _mapper.ToGetResponse(entity);
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized FileHub Category Get Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
        return null;
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    public partial GetCategoryDto ToGetResponse(Persistence.Entities.Category category);
}
