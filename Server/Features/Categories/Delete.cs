// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record DeleteCategoryRequest : RequestBase, IRequest<int>
{
    public int Id { get; set; }
}

public class DeleteHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<DeleteCategoryRequest, int>
{
    public async Task<int> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized FileHub Category Delete Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return -1;
        }

        using var db = CreateDbContext();
        var rowsAffected = await db.Category
            .Where(c => c.Id == request.Id && c.ModuleId == request.ModuleId)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected > 0)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Delete, "FileHub Category Deleted {Id}", request.Id);
            return request.Id;
        }

        Logger.Log(LogLevel.Warning, this, LogFunction.Delete, "FileHub Category Not Found {Id}", request.Id);
        return -1;
    }
}
