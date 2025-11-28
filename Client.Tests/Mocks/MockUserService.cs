// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

/// <summary>
/// Mock implementation of IUserService for testing.
/// Provides basic user functionality without making actual API calls.
/// </summary>
public class MockUserService : IUserService
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public MockUserService()
    {
        // Add a default test user
        _users.Add(new User
        {
            UserId = 1,
            Username = "testuser",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsDeleted = false
        });
        _nextId = 2;
    }

    public Task<User> GetUserAsync(int userId, int siteId)
    {
        var user = _users.FirstOrDefault(u => u.UserId == userId);
        return Task.FromResult(user ?? new User());
    }

    public Task<User> GetUserAsync(string username, int siteId)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user ?? new User());
    }

    public Task<User> GetUserAsync(string username, string email, int siteId)
    {
        var user = _users.FirstOrDefault(u => u.Username == username || u.Email == email);
        return Task.FromResult(user ?? new User());
    }

    public Task<List<User>> GetUsersAsync(int siteId)
    {
        return Task.FromResult(_users.Where(u => !u.IsDeleted).ToList());
    }

    public Task<User> AddUserAsync(User user, Alias alias)
    {
        user.UserId = _nextId++;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User> AddUserAsync(User user)
    {
        user.UserId = _nextId++;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User> UpdateUserAsync(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.UserId == user.UserId);
        if (existingUser != null)
        {
            _users.Remove(existingUser);
            _users.Add(user);
        }
        return Task.FromResult(user);
    }

    public Task DeleteUserAsync(int userId)
    {
        var user = _users.FirstOrDefault(u => u.UserId == userId);
        if (user != null)
        {
            user.IsDeleted = true;
        }
        return Task.CompletedTask;
    }

    public Task DeleteUserAsync(int userId, int siteId)
    {
        var user = _users.FirstOrDefault(u => u.UserId == userId);
        if (user != null)
        {
            user.IsDeleted = true;
        }
        return Task.CompletedTask;
    }

    public Task<User> LoginUserAsync(User user, bool setCookie, bool isPersistent)
    {
        return Task.FromResult(user);
    }

    public Task LogoutUserAsync(User user)
    {
        return Task.CompletedTask;
    }

    public Task LogoutUserEverywhereAsync(User user)
    {
        return Task.CompletedTask;
    }

    public Task<User> VerifyEmailAsync(User user, string token)
    {
        return Task.FromResult(user);
    }

    public Task<User> VerifyTwoFactorAsync(User user, string token)
    {
        return Task.FromResult(user);
    }

    public Task ForgotPasswordAsync(User user)
    {
        return Task.CompletedTask;
    }

    public Task<User> ResetPasswordAsync(User user, string token)
    {
        return Task.FromResult(user);
    }

    public Task<Dictionary<string, string>> ImportUsersAsync(int siteId, int fileId)
    {
        return Task.FromResult(new Dictionary<string, string>());
    }

    public Task<Dictionary<string, string>> ImportUsersAsync(int siteId, int fileId, bool notify)
    {
        return Task.FromResult(new Dictionary<string, string>());
    }

    public Task<User> LinkExternalAccountAsync(User user, string token, string returnUrl)
    {
        return Task.FromResult(user);
    }

    public Task<User> LinkUserAsync(User user, string username, string password, string token, string returnUrl)
    {
        return Task.FromResult(user);
    }

    public Task<byte[]> CreateGraphAsync(int siteId, List<int> userIds)
    {
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<bool> ValidatePasswordAsync(string password)
    {
        return Task.FromResult(true);
    }

    public Task<UserValidateResult> ValidateUserAsync(string username, string password, string twoFactorCode)
    {
        // Return a simple default result - Oqtane's UserValidateResult structure may vary
        return Task.FromResult(new UserValidateResult());
    }

    public Task<string> GetTokenAsync()
    {
        return Task.FromResult("mock-token");
    }

    public Task<string> GetPersonalAccessTokenAsync()
    {
        return Task.FromResult("mock-pat-token");
    }

    public Task<string> GetPasswordRequirementsAsync(int siteId)
    {
        return Task.FromResult("Minimum 6 characters");
    }
}
