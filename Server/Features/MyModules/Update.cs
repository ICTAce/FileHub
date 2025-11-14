namespace ICTAce.FileHub.Features.MyModules;

// Command
public class UpdateMyModuleCommand : IRequest<Models.MyModule>
{
    public Models.MyModule MyModule { get; set; }
}

// Handler
public class UpdateHandler : CommandHandlerBase, IRequestHandler<UpdateMyModuleCommand, Models.MyModule>
{
    public UpdateHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<Models.MyModule> Handle(UpdateMyModuleCommand request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.MyModule.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();
            db.Entry(request.MyModule).State = EntityState.Modified;
            await db.SaveChangesAsync(cancellationToken);
            
            Logger.Log(LogLevel.Information, this, LogFunction.Update, "MyModule Updated {MyModule}", request.MyModule);
            return request.MyModule;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Update Attempt {MyModule}", request.MyModule);
            return null;
        }
    }
}
