namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string NumGroup { get; set; } = string.Empty;
    public string NumRoom { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class AuthToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
