// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Base handler class for command operations (Create, Update, Delete).
/// Inherits common infrastructure from HandlerBase and provides command-specific context.
/// Follows CQRS principles by using MyModuleCommandContext for write operations.
/// </summary>
public abstract class CommandHandlerBase(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    : HandlerBase<ApplicationCommandContext>(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger);
