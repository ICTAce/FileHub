// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence;

public class ApplicationCommandContext(
    IDBContextDependencies DBContextDependencies)
    : ApplicationContext(DBContextDependencies);
