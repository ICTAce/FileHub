// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.MyModules;

public class ListHandler(
    IDbContextFactory<MyModuleCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : QueryHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<ListMyModuleRequest, PagedResult<ListMyModuleResponse>>
{
    public async Task<PagedResult<ListMyModuleResponse>> Handle(ListMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();

            // Get total count for pagination metadata
            var totalCount = await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .CountAsync(cancellationToken).ConfigureAwait(false);

            // Apply pagination
            var modules = await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .OrderBy(m => m.Name) // Consistent ordering for pagination
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            // Map MyModule entities to ListMyModuleResponse DTOs using Mapperly
            var items = modules
                .Select(Mapper.ToListResponse)
                .ToList();

            return new PagedResult<ListMyModuleResponse>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {ModuleId}", request.ModuleId);
            return null;
        }
    }
}
