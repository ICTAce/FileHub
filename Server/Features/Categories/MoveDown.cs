// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record MoveDownCategoryRequest : EntityRequestBase, IRequest<int>;

public class MoveDownHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<MoveDownCategoryRequest, int>
{
    public async Task<int> Handle(MoveDownCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, 
                "Unauthorized Category MoveDown Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
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

        var nextCategory = await db.Category
            .Where(c => c.ModuleId == request.ModuleId 
                     && c.ParentId == currentCategory.ParentId
                     && c.ViewOrder > currentCategory.ViewOrder)
            .OrderBy(c => c.ViewOrder)
            .Select(c => new { c.Id, c.ViewOrder })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (nextCategory is null)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Update, 
                "Category Already at Bottom {Id}", request.Id);
            return request.Id;
        }

        var currentViewOrder = currentCategory.ViewOrder;
        var nextViewOrder = nextCategory.ViewOrder;

        await db.Category
            .Where(c => c.Id == currentCategory.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.ViewOrder, nextViewOrder), cancellationToken)
            .ConfigureAwait(false);

        await db.Category
            .Where(c => c.Id == nextCategory.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(c => c.ViewOrder, currentViewOrder), cancellationToken)
            .ConfigureAwait(false);

        Logger.Log(LogLevel.Information, this, LogFunction.Update, 
            "Category Moved Down {Id}", request.Id);

        return request.Id;
    }
}
