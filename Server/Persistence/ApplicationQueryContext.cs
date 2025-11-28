// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence;

public class ApplicationQueryContext(
    IDBContextDependencies DBContextDependencies)
    : ApplicationContext(DBContextDependencies: DBContextDependencies)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // FIXME: ApplicationQueryContext supposed to be read-only by using query connection string
        var tenant = DBContextDependencies.TenantManager.GetTenant();

        if (tenant != null)
        {
            var queryConnectionString = DBContextDependencies.Config.GetConnectionString(tenant.DBConnectionString + "_Query");
            if (!string.IsNullOrEmpty(queryConnectionString))
            {
                queryConnectionString = queryConnectionString.Replace($"|{Constants.DataDirectory}|", AppDomain.CurrentDomain.GetData(Constants.DataDirectory)?.ToString());
                var databaseType = tenant.DBType;

                if (!string.IsNullOrEmpty(databaseType))
                {
                    var type = Type.GetType(databaseType);
                    ActiveDatabase = Activator.CreateInstance(type!) as IDatabase;

                    if (ActiveDatabase != null)
                    {
                        optionsBuilder.UseOqtaneDatabase(ActiveDatabase, queryConnectionString);
                    }
                }
            }
        }

        base.OnConfiguring(optionsBuilder);
    }
}
