// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

// Handler
public class ListHandler(
    IDbContextFactory<Context> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<ListMyModulesRequest, PagedResult<ListMyModulesResponse>>
{
    public async Task<PagedResult<ListMyModulesResponse>> Handle(ListMyModulesRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();

            // Get total count for pagination metadata
            var totalCount = await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .CountAsync(cancellationToken);

            // Apply pagination
            var modules = await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .OrderBy(m => m.Name) // Consistent ordering for pagination
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Project MyModule entities to ListMyModulesResponse DTOs
            var items = modules
                .Select(m => new ListMyModulesResponse 
                { 
                    Id = m.Id,
                    Name = m.Name 
                })
                .ToList();

            return new PagedResult<ListMyModulesResponse>
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
