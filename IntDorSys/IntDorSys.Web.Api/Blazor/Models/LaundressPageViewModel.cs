namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class LaundressPageViewModel
{
    public string Date { get; set; } = string.Empty;
    public List<LaundressViewModel> LaundModels { get; set; } = [];
}
