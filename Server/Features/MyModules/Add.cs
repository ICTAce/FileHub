namespace ICTAce.FileHub.Features.MyModules;

// Command
public class AddMyModuleCommand : IRequest<Models.MyModule>
{
    public Models.MyModule MyModule { get; set; }
}

// Handler
public class AddHandler : CommandHandlerBase, IRequestHandler<AddMyModuleCommand, Models.MyModule>
{
    public AddHandler(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
        : base(contextFactory, userPermissions, tenantManager, httpContextAccessor, logger)
    {
    }

    public async Task<Models.MyModule> Handle(AddMyModuleCommand request, CancellationToken cancellationToken)
    {
        var alias = GetAlias();
        
        if (IsAuthorized(alias.SiteId, request.MyModule.ModuleId, PermissionNames.Edit))
        {
            using var db = CreateDbContext();
            db.MyModule.Add(request.MyModule);
            await db.SaveChangesAsync(cancellationToken);
            
            Logger.Log(LogLevel.Information, this, LogFunction.Create, "MyModule Added {MyModule}", request.MyModule);
            return request.MyModule;
        }
        else
        {
            Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized MyModule Add Attempt {MyModule}", request.MyModule);
            return null;
        }
    }
}
