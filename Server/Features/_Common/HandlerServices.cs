// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Encapsulates common dependencies required by handlers.
/// Generic service container that works with any ApplicationContext type.
/// Reduces constructor parameter repetition and centralizes infrastructure concerns.
/// </summary>
/// <typeparam name="TContext">The type of ApplicationContext (Query or Command)</typeparam>
public class HandlerServices<TContext>(
    IDbContextFactory<TContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    where TContext : ApplicationContext
{
    public IDbContextFactory<TContext> ContextFactory { get; } = contextFactory;
    public IUserPermissions UserPermissions { get; } = userPermissions;
    public ITenantManager TenantManager { get; } = tenantManager;
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    public ILogManager Logger { get; } = logger;
}
