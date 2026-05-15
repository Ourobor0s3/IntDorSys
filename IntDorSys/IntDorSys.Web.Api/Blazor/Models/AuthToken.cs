namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class AuthToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string? Role { get; set; }
}
