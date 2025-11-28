// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Helpers;

public static class TestHelpers
{
    public static IHttpContextAccessor CreateMockHttpContextAccessor(ClaimsPrincipal user)
    {
        var mockHttpContext = Substitute.For<HttpContext>();
        mockHttpContext.User.Returns(user);

        var mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        mockHttpContextAccessor.HttpContext.Returns(mockHttpContext);

        return mockHttpContextAccessor;
    }

    public static ClaimsPrincipal CreateTestUser(int userId = 1, string userName = "testuser")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, userName)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }
}
