// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockLogService : ILogService
{
    public Task DeleteLogsAsync(int siteId) => Task.CompletedTask;

    public Task<Log> GetLogAsync(int logId) => Task.FromResult(new Log());

    public Task<List<Log>> GetLogsAsync(int siteId, string level, string function, int rows) => Task.FromResult(new List<Log>());

    public Task Log(int? pageId, int? moduleId, int? userId, string category, string feature, LogFunction function, LogLevel level, Exception exception, string message, params object[] args)
    => Task.CompletedTask;

    public Task Log(Alias alias, int? pageId, int? moduleId, int? userId, string category, string feature, LogFunction function, LogLevel level, Exception exception, string message, params object[] args)
    => Task.CompletedTask;
}
