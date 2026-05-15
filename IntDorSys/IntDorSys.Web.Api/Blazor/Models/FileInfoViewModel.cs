namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class FileInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public Guid Guid { get; set; }
    public string? FilePath { get; set; }
    public string? GroupId { get; set; }
    public string? Description { get; set; }
}
