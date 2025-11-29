// Licensed to ICTAce under the MIT license.

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "Test User"),
        }, "Test");
        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }
}
