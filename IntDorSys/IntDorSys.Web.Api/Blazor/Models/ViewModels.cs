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

public sealed class LaundressPageViewModel
{
    public string Date { get; set; } = string.Empty;
    public List<LaundressViewModel> LaundModels { get; set; } = [];
}

public sealed class LaundressViewModel
{
    public UserInfoViewModel? SelectUser { get; set; }
    public DateTime TimeWash { get; set; }
}

public sealed class ReportViewModel
{
    public long UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Username { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<FileInfoViewModel> Files { get; set; } = [];
}

public sealed class FileInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public Guid Guid { get; set; }
    public string? FilePath { get; set; }
    public string? GroupId { get; set; }
    public string? Description { get; set; }
}

public sealed class ChartPoint
{
    public string Name { get; set; } = string.Empty;
    public int Value1 { get; set; }
    public int Value2 { get; set; }
}
