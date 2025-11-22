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
        TestContext.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
        TestContext.Services.AddSingleton(new SiteState());
        TestContext.Services.AddLogging();
        TestContext.Services.AddScoped<ILogService, MockLogService>();
        TestContext.Services.AddScoped<ISampleModuleService, MockSampleModuleService>();
        
        // Add PageState mock for cascading parameter
        TestContext.Services.AddScoped(_ => new Mocks.PageState
        {
            Action = "Index",
            QueryString = []
        });
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
