// Licensed to ICTAce under the MIT license.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockNavigationManager : NavigationManager
{
    public bool NavigateToInvoked { get; private set; }
    public string? LastNavigatedUri { get; private set; }

    public MockNavigationManager()
    {
        base.Initialize("https://localhost:5001/", "https://localhost:5001/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        NavigateToInvoked = true;
        LastNavigatedUri = uri;
        // Mock implementation - just update the Uri property through reflection
        base.NavigateTo(uri, forceLoad);
    }

    protected override void NavigateToCore(string uri, NavigationOptions options)
    {
        NavigateToInvoked = true;
        LastNavigatedUri = uri;
        base.NavigateTo(uri, options);
    }

    public void Reset()
    {
        NavigateToInvoked = false;
        LastNavigatedUri = null;
    }
}
