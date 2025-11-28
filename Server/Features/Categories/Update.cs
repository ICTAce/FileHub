// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Categories;

public record UpdateCategoryRequest : RequestBase, IRequest<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }
}

public class UpdateHandler(HandlerServices<ApplicationCommandContext> services)
    : HandlerBase<ApplicationCommandContext>(services), IRequestHandler<UpdateCategoryRequest, int>
{
    public async Task<int> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();

            var category = await db.Category.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            if (category != null)
            {
                category.Name = request.Name;
                category.ViewOrder = request.ViewOrder;
                category.ParentId = request.ParentId;

                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                Logger.Log(LogLevel.Information, this, LogFunction.Update, "FileHub Category Updated {Category}", category);
                return request.Id;
            }

            return -1;
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized FileHub Category Update Attempt {Id}", request.Id);
        return -1;
    }
}
