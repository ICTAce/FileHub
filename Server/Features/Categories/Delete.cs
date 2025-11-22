// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Categories;

public record DeleteCategoryRequest : RequestBase, IRequest<int>
{
    public int Id { get; set; }
}

public class DeleteHandler(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<DeleteCategoryRequest, int>
{
    public async Task<int> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();
            var category = await db.Category.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            if (category != null)
            {
                db.Category.Remove(category);
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                Logger.Log(LogLevel.Information, this, LogFunction.Delete, "Category Deleted {Id}", request.Id);
                return request.Id;
            }
            return -1;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Delete Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return -1;
        }
    }
}
