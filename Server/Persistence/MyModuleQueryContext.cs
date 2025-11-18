// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence;

public class MyModuleQueryContext : MyModuleContext
{
    public MyModuleQueryContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }
}
