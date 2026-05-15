namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class ReportViewModel
{
    public long UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Username { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<FileInfoViewModel> Files { get; set; } = [];
}
