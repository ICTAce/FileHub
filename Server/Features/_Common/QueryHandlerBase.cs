// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Common;

/// <summary>
/// Base handler class for query operations (List, Get).
/// Inherits common infrastructure from HandlerBase and provides query-specific context.
/// Follows CQRS principles by using MyModuleQueryContext for read-only operations.
/// </summary>
public abstract class QueryHandlerBase(
    IDbContextFactory<ApplicationQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : HandlerBase<ApplicationQueryContext>(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger);
