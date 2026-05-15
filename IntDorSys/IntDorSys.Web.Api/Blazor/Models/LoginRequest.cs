namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
