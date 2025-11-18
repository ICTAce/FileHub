// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests;

public abstract class BaseTest : IDisposable
{
    private bool _disposed;
    protected BunitContext TestContext { get; private set; }

    protected BaseTest()
    {
        TestContext = new();
        TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.formValid", _ => true);
        TestContext.Services.AddLocalization();
        TestContext.Services.AddSingleton<Microsoft.Extensions.Localization.IStringLocalizerFactory, MockStringLocalizerFactory>();
        TestContext.Services.AddScoped<IMyModuleService, MockMyModuleService>();
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
