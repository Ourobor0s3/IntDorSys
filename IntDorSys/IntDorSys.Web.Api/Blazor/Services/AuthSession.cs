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

        try
        {
            // 1) Try init auth data injected by server from cookie (page load/refresh)
            var init = await js.InvokeAsync<InitAuthData?>("__getInitAuth");
            if (init is { Token.Length: > 0 })
            {
                AccessToken = init.Token;
                UserRole = string.IsNullOrWhiteSpace(init.Role) ? null : init.Role;
                await js.InvokeVoidAsync("__clearInitAuth");
                _initialized = true;
                return;
            }

            // 2) Fallback: read from localStorage
            var token = await js.InvokeAsync<string?>("__storage.get", TokenKey);
            var role = await js.InvokeAsync<string?>("__storage.get", RoleKey);
            AccessToken = string.IsNullOrWhiteSpace(token) ? null : token;
            UserRole = string.IsNullOrWhiteSpace(role) ? null : role;
        }
        catch
        {
        }
        finally
        {
            _initialized = true;
            _initializing = false;
        }
    }

    public async Task LoginAsync(string accessToken, string? userRole = null)
    {
        AccessToken = accessToken;
        UserRole = userRole;

        try
        {
            await js.InvokeVoidAsync("__storage.set", TokenKey, accessToken);
            await js.InvokeVoidAsync("__cookie.set", TokenKey, accessToken, 30);
            if (!string.IsNullOrWhiteSpace(userRole))
            {
                await js.InvokeVoidAsync("__storage.set", RoleKey, userRole);
                await js.InvokeVoidAsync("__cookie.set", RoleKey, userRole, 30);
            }
        }
        catch
        {
        }
    }

    public async Task LogoutAsync()
    {
        AccessToken = null;
        UserRole = null;

        try
        {
            await js.InvokeVoidAsync("__storage.remove", TokenKey);
            await js.InvokeVoidAsync("__storage.remove", RoleKey);
            await js.InvokeVoidAsync("__cookie.remove", TokenKey);
            await js.InvokeVoidAsync("__cookie.remove", RoleKey);
        }
        catch
        {
        }
    }

    private sealed class InitAuthData
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
