// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

// Handler
public class ListHandler(
    IDbContextFactory<Context> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<ListMyModulesRequest, List<ListMyModulesResponse>>
{
    public async Task<List<ListMyModulesResponse>> Handle(ListMyModulesRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.View))
        {
            using var db = CreateDbContext();
            var modules = await db.MyModule
                .Where(item => item.ModuleId == request.ModuleId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Project MyModule entities to ListMyModulesResponse DTOs
            return modules
                .Select(m => new ListMyModulesResponse 
                { 
                    Id = m.Id,
                    Name = m.Name 
                })
                .ToList();
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Get Attempt {ModuleId}", request.ModuleId);
            return null;
        }
    }
}
