// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

public abstract class QueryHandlerBase(
    IDbContextFactory<ApplicationQueryContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : HandlerBase<ApplicationQueryContext>(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger);
