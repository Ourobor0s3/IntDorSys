using System.Text.Json;
using Microsoft.JSInterop;

namespace IntDorSys.Web.Api.Blazor.Services;

public sealed class AuthSession(IJSRuntime js)
{
    private const string TokenKey = "auth_token";
    private const string RoleKey = "user_role";
    private bool _initialized;
    private bool _initializing;

    public string? AccessToken { get; private set; }
    public string? UserRole { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(AccessToken);
    public bool IsAdmin => UserRole == "admin";

    public async Task InitializeAsync()
    {
        if (_initialized || _initializing) return;
        _initializing = true;

        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
                var role = await js.InvokeAsync<string?>("localStorage.getItem", RoleKey);
                AccessToken = string.IsNullOrWhiteSpace(token) ? null : token;
                UserRole = string.IsNullOrWhiteSpace(role) ? null : role;
                break;
            }
            catch when (attempt < 2)
            {
                await Task.Delay(100 * (attempt + 1));
            }
            catch
            {
                break;
            }
        }

        _initialized = true;
        _initializing = false;
    }

    public async Task LoginAsync(string accessToken, string? userRole = null)
    {
        AccessToken = accessToken;
        UserRole = userRole;
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", TokenKey, accessToken);
            if (!string.IsNullOrWhiteSpace(userRole))
            {
                await js.InvokeVoidAsync("localStorage.setItem", RoleKey, userRole);
            }
        }
        catch
        {
            // Ignore
        }
    }

    public async Task LogoutAsync()
    {
        AccessToken = null;
        UserRole = null;
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            await js.InvokeVoidAsync("localStorage.removeItem", RoleKey);
        }
        catch
        {
            // Ignore
        }
    }

    public void Login(string accessToken, string? userRole = null) => LoginAsync(accessToken, userRole).Wait();
    public void Logout() => LogoutAsync().Wait();
}