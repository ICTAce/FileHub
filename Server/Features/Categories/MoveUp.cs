// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record MoveUpCategoryRequest : EntityRequestBase, IRequest<int>;

public class MoveUpHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<MoveUpCategoryRequest, int>
{
    public async Task<int> Handle(MoveUpCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security,
                "Unauthorized Category MoveUp Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return -1;
        }

        using var db = CreateDbContext();

        var currentCategory = await db.Category
            .Where(c => c.Id == request.Id && c.ModuleId == request.ModuleId)
            .Select(c => new { c.Id, c.ViewOrder, c.ParentId })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (currentCategory is null)
        {
            Logger.Log(LogLevel.Warning, this, LogFunction.Update,
                "Category Not Found {Id}", request.Id);
            return -1;
        }

        var previousCategory = await db.Category
            .Where(c => c.ModuleId == request.ModuleId
                     && c.ParentId == currentCategory.ParentId
                     && c.ViewOrder < currentCategory.ViewOrder)
            .OrderByDescending(c => c.ViewOrder)
            .Select(c => new { c.Id, c.ViewOrder })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (previousCategory is null)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Update,
                "Category Already at Top {Id}", request.Id);
            return request.Id;
        }

        var currentViewOrder = currentCategory.ViewOrder;
        var previousViewOrder = previousCategory.ViewOrder;

        await db.Category
            .Where(c => c.Id == currentCategory.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.ViewOrder, previousViewOrder), cancellationToken)
            .ConfigureAwait(false);

        await db.Category
            .Where(c => c.Id == previousCategory.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.ViewOrder, currentViewOrder), cancellationToken)
            .ConfigureAwait(false);

        Logger.Log(LogLevel.Information, this, LogFunction.Update,
            "Category Moved Up {Id}", request.Id);

        return request.Id;
    }
}
