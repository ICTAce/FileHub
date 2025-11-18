// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence;

public class MyModuleCommandContext : MyModuleContext
{
    public MyModuleCommandContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }
}
