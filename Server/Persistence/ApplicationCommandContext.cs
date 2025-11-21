// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence;

public class ApplicationCommandContext : ApplicationContext
{
    public ApplicationCommandContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }
}
