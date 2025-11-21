// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Categories;

public class CreateHandler(
    IDbContextFactory<MyModuleCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<CreateCategoryRequest, int>
{
    public async Task<int> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            // Build the entity from command data
            var category = new Persistence.Entities.Category
            {
                ModuleId = request.ModuleId,
                Name = request.Name,
                ViewOrder = request.ViewOrder,
                ParentId = request.ParentId
                // CreatedBy, CreatedOn, ModifiedBy, ModifiedOn will be set by IAuditable/database
            };

            using var db = CreateDbContext();
            db.Category.Add(category);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.Log(LogLevel.Information, this, LogFunction.Create, "Category Added {Category}", category);
            return category.Id;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Add Attempt {ModuleId} {Name}", request.ModuleId, request.Name);
            return -1;
        }
    }
}
