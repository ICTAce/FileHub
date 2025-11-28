// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Encapsulates common dependencies required by handlers.
/// Generic service container that works with any ApplicationContext type.
/// Reduces constructor parameter repetition and centralizes infrastructure concerns.
/// </summary>
/// <typeparam name="TContext">The type of ApplicationContext (Query or Command)</typeparam>
public class HandlerServices<TContext>
    where TContext : ApplicationContext
{
    public HandlerServices(
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

    public IDbContextFactory<TContext> ContextFactory { get; }
    public IUserPermissions UserPermissions { get; }
    public ITenantManager TenantManager { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public ILogManager Logger { get; }
}
