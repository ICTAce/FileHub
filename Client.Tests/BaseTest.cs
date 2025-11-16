using System;
using ICTAce.FileHub.Services;

namespace ICTAce.FileHub.Client.Tests;
public abstract class BaseTest : IDisposable
{
    private bool _disposed;
    protected BunitContext TestContext { get; private set; }

    protected BaseTest()
    {
        TestContext = new();

        TestContext.Services.AddScoped<IMyModuleService, MyModuleService>();
        TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                TestContext.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
