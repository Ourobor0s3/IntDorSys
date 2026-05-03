using System.Text.Json;
using Microsoft.JSInterop;

namespace IntDorSys.Web.Api.Blazor.Services;

public sealed class AuthSession(IJSRuntime js)
{
    private const string TokenKey = "auth_token";
    private bool _initialized;

    public string? AccessToken { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(AccessToken);

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;

        try
        {
            var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            if (!string.IsNullOrWhiteSpace(token))
            {
                AccessToken = token;
            }
        }
        catch
        {
            // Ignore - running without JS
        }
    }

    public async Task LoginAsync(string accessToken)
    {
        AccessToken = accessToken;
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", TokenKey, accessToken);
        }
        catch
        {
            // Ignore
        }
    }

    public async Task LogoutAsync()
    {
        AccessToken = null;
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
        catch
        {
            // Ignore
        }
    }

    public void Login(string accessToken) => LoginAsync(accessToken).Wait();
    public void Logout() => LogoutAsync().Wait();
}