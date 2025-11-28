// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

public abstract class HandlerBase<TContext>
    where TContext : ApplicationContext
{
    protected readonly IDbContextFactory<TContext> ContextFactory;
    protected readonly IUserPermissions UserPermissions;
    protected readonly ITenantManager TenantManager;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly ILogManager Logger;

    // New pattern: Generic service parameter (recommended)
    protected HandlerBase(HandlerServices<TContext> services)
    {
        ContextFactory = services.ContextFactory;
        UserPermissions = services.UserPermissions;
        TenantManager = services.TenantManager;
        HttpContextAccessor = services.HttpContextAccessor;
        Logger = services.Logger;
    }

    // Legacy constructor for backward compatibility
    protected HandlerBase(
        IDbContextFactory<TContext> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
    {
        ContextFactory = contextFactory;
        UserPermissions = userPermissions;
        TenantManager = tenantManager;
        HttpContextAccessor = httpContextAccessor;
        Logger = logger;
    }

    protected Alias GetAlias() => TenantManager.GetAlias();

    protected ClaimsPrincipal? GetCurrentUser() => HttpContextAccessor.HttpContext?.User;

    protected bool IsAuthorized(int siteId, int moduleId, string permission)
    {
        var user = GetCurrentUser();
        return user != null && UserPermissions.IsAuthorized(user, siteId, EntityNames.Module, moduleId, permission);
    }

    protected TContext CreateDbContext() => ContextFactory.CreateDbContext();

    /// <summary>
    /// Generic handler for creating entities with authorization and logging.
    /// </summary>
    /// <typeparam name="TRequest">The request type containing the data (must have ModuleId property)</typeparam>
    /// <typeparam name="TEntity">The entity type to create (must inherit from AuditableBase)</typeparam>
    /// <param name="request">The request containing the data and ModuleId</param>
    /// <param name="mapToEntity">Mapper function to convert request to entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created entity ID on success, -1 on authorization failure</returns>
    protected async Task<int> HandleCreateAsync<TRequest, TEntity>(
        TRequest request,
        Func<TRequest, TEntity> mapToEntity,
        CancellationToken cancellationToken = default)
        where TEntity : AuditableBase
        where TRequest : notnull
    {
        // Extract ModuleId from request - must be RequestBase or have ModuleId property
        if (request is not RequestBase requestBase)
        {
            throw new InvalidOperationException($"Request type {typeof(TRequest).Name} must inherit from RequestBase to have ModuleId property");
        }

        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, requestBase.ModuleId, PermissionNames.Edit))
        {
            var entity = mapToEntity(request);

            using var db = CreateDbContext();
            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.Log(LogLevel.Information, this, LogFunction.Create, "{EntityName} Added {Entity}", typeof(TEntity).Name, entity);
            return entity.Id;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized {EntityName} Add Attempt {ModuleId}", typeof(TEntity).Name, requestBase.ModuleId);
            return -1;
        }
    }

    /// <summary>
    /// Generic handler for deleting entities with authorization and logging.
    /// </summary>
    /// <typeparam name="TRequest">The request type containing Id and ModuleId (must inherit from EntityRequestBase)</typeparam>
    /// <typeparam name="TEntity">The entity type to delete (must inherit from AuditableModuleBase)</typeparam>
    /// <param name="request">The request containing the entity Id and ModuleId</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deleted entity ID on success, -1 on authorization failure or not found</returns>
    protected async Task<int> HandleDeleteAsync<TRequest, TEntity>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : EntityRequestBase
        where TEntity : AuditableModuleBase
    {
        var alias = GetAlias();

        if (!IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized {EntityName} Delete Attempt {Id} {ModuleId}", typeof(TEntity).Name, request.Id, request.ModuleId);
            return -1;
        }

        using var db = CreateDbContext();
        var rowsAffected = await db.Set<TEntity>()
            .Where(e => e.Id == request.Id && e.ModuleId == request.ModuleId)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        if (rowsAffected > 0)
        {
            Logger.Log(LogLevel.Information, this, LogFunction.Delete, "{EntityName} Deleted {Id}", typeof(TEntity).Name, request.Id);
            return request.Id;
        }

        Logger.Log(LogLevel.Warning, this, LogFunction.Delete, "{EntityName} Not Found {Id}", typeof(TEntity).Name, request.Id);
        return -1;
    }
}
