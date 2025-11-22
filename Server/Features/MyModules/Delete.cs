// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.MyModules;

public record DeleteMyModuleRequest : RequestBase, IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}

/// <summary>
/// Handles the delete command for a MyModule aggregate. Returns the deleted Id or -1 if not found/unauthorized.
/// </summary>
public class DeleteHandler(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : CommandHandlerBase(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger), IRequestHandler<DeleteMyModuleRequest, int>
{
    public async Task<int> Handle(DeleteMyModuleRequest request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();

        // Enforce authorization at the aggregate boundary
        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {Id} {ModuleId}", request.Id, request.ModuleId);
            return -1;
        }

        using var db = CreateDbContext();
        // Use ExecuteDeleteAsync for efficient direct deletion
        var rowsAffected = await db.MyModule
            .Where(m => m.Id == request.Id && m.ModuleId == request.ModuleId)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected > 0)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Delete, "MyModule Deleted {Id}", request.Id);
            return request.Id;
        }
        else
        {
            Logger.Log(LogLevel.Warning, this, LogFunction.Delete, "MyModule Not Found {Id}", request.Id);
            return -1;
        }
    }
}
