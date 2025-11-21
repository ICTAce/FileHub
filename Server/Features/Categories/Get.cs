// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Categories;

public class GetHandler(
    IDbContextFactory<MyModuleQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<GetCategoryRequest, GetCategoryResponse?>
{
    private static readonly GetMapper _mapper = new();

    public async Task<GetCategoryResponse?> Handle(GetCategoryRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var entity = await db.Category.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            if (entity is null)
            {
                Logger.Log(LogLevel.Error, this, LogFunction.Security, "Category not found {Id} {ModuleId}", request.Id, request.ModuleId);
                return null;
            }

            return _mapper.ToGetResponse(entity);
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Category Get Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return null;
        }
    }
}

[Mapper]
internal sealed partial class GetMapper
{
    /// <summary>
    /// Maps Category entity to GetCategoryResponse DTO
    /// </summary>
    public partial GetCategoryResponse ToGetResponse(Persistence.Entities.Category category);
}
