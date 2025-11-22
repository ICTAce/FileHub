// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

public abstract class CommandHandlerBase(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : HandlerBase<ApplicationCommandContext>(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger);
