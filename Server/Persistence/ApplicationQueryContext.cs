// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence;

public class ApplicationQueryContext : ApplicationContext
{
    public ApplicationQueryContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }
}
