// Licensed to ICTAce under the MIT license.

using Oqtane.Enums;
using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

/// <summary>
/// Mock implementation of ILogService for testing.
/// Captures log entries for verification and prevents exceptions.
/// </summary>
public class MockLogService : ILogService
{
    private readonly List<LogEntry> _logEntries = [];

    public IReadOnlyList<LogEntry> LogEntries => _logEntries.AsReadOnly();

    public void ClearLogs() => _logEntries.Clear();

    public Task DeleteLogsAsync(int siteId) => Task.CompletedTask;

    public Task<Log> GetLogAsync(int logId) => Task.FromResult(new Log());

    public Task<List<Log>> GetLogsAsync(int siteId, string level, string function, int rows) 
        => Task.FromResult(new List<Log>());

    public Task Log(int? pageId, int? moduleId, int? userId, string category, string feature, LogFunction function, Oqtane.Shared.LogLevel level, Exception? exception, string message, params object[] args)
    {
        _logEntries.Add(new LogEntry
        {
            PageId = pageId,
            ModuleId = moduleId,
            UserId = userId,
            Category = category,
            Feature = feature,
            Function = function,
            Level = level,
            Exception = exception,
            Message = message,
            Args = args,
            Timestamp = DateTime.UtcNow
        });
        
        return Task.CompletedTask;
    }

    public Task Log(Alias? alias, int? pageId, int? moduleId, int? userId, string category, string feature, LogFunction function, Oqtane.Shared.LogLevel level, Exception? exception, string message, params object[] args)
    {
        _logEntries.Add(new LogEntry
        {
            Alias = alias,
            PageId = pageId,
            ModuleId = moduleId,
            UserId = userId,
            Category = category,
            Feature = feature,
            Function = function,
            Level = level,
            Exception = exception,
            Message = message,
            Args = args,
            Timestamp = DateTime.UtcNow
        });
        
        return Task.CompletedTask;
    }

    public class LogEntry
    {
        public Alias? Alias { get; init; }
        public int? PageId { get; init; }
        public int? ModuleId { get; init; }
        public int? UserId { get; init; }
        public string Category { get; init; } = string.Empty;
        public string Feature { get; init; } = string.Empty;
        public LogFunction Function { get; init; }
        public Oqtane.Shared.LogLevel Level { get; init; }
        public Exception? Exception { get; init; }
        public string Message { get; init; } = string.Empty;
        public object[] Args { get; init; } = [];
        public DateTime Timestamp { get; init; }
    }
}
