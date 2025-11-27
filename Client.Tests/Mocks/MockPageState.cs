// Licensed to ICTAce under the MIT license.

using Oqtane.Models;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class PageState
{
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, string> QueryString { get; set; } = [];
    public string Url { get; set; } = "/";
    public int PageId { get; set; } = 1;
    public string Path { get; set; } = "/";
    public string ReturnUrl { get; set; } = string.Empty;
    public int ModuleId { get; set; }
    public int UserId { get; set; } = -1;
    public string? RemoteIPAddress { get; set; }
    public Page? Page { get; set; }
    public List<Module> Modules { get; set; } = [];
    public Alias? Alias { get; set; }
    public Site? Site { get; set; }
    public User? User { get; set; }
    public string EditMode { get; set; } = string.Empty;
    public DateTime LastSyncDate { get; set; } = DateTime.UtcNow;
    public string Runtime { get; set; } = "WebAssembly";
    public int VisitorId { get; set; }
    public string UrlParameters { get; set; } = string.Empty;
    public string QueryString2 { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public string Control { get; set; } = string.Empty;
    public bool IsNavigating { get; set; }
    public bool Refresh { get; set; }
}
