namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class UserInfoViewModel
{
    public long Id { get; set; }
    public string? FullName { get; set; }
    public string? NumGroup { get; set; }
    public string? NumRoom { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Status { get; set; }
    public string RegisterDate { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public bool IsConfirm { get; set; }
}
