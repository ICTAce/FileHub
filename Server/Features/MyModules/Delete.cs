namespace ICTAce.FileHub.Features.MyModules;

// Command
public class DeleteMyModuleCommand : IRequest<Unit>
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
}

// Handler
public class DeleteHandler : CommandHandlerBase, IRequestHandler<DeleteMyModuleCommand, Unit>
{
    public DeleteHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<Unit> Handle(DeleteMyModuleCommand request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();
            var myModule = await db.MyModule.FindAsync([request.MyModuleId], cancellationToken);
            if (myModule != null)
            {
                db.MyModule.Remove(myModule);
                await db.SaveChangesAsync(cancellationToken);
                Logger.Log(LogLevel.Information, this, LogFunction.Delete, "MyModule Deleted {MyModuleId}", request.MyModuleId);
            }
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Delete Attempt {MyModuleId} {ModuleId}", request.MyModuleId, request.ModuleId);
        }
        
        return Unit.Value;
    }
}
