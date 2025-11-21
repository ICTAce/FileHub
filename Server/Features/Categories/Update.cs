// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Categories;

// Handler
public class UpdateHandler(
    IDbContextFactory<MyModuleCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<UpdateCategoryRequest, int>
{
    public async Task<int> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();

            // Fetch existing entity
            var category = await db.Category.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            if (category != null)
            {
                // Update only user-editable fields
                category.Name = request.Name;
                category.ViewOrder = request.ViewOrder;
                category.ParentId = request.ParentId;
                // ModifiedBy, ModifiedOn will be updated by IAuditable/database

                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                Logger.Log(LogLevel.Information, this, LogFunction.Update, "Category Updated {Category}", category);
                return request.Id;
            }

            return -1;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Update Attempt {Id}", request.Id);
            return -1;
        }
    }
}
