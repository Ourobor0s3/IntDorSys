using System.Text.Json.Serialization;

namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class ApiResponse<T>
{
    public T? Data { get; set; }
    [JsonConverter(typeof(ApiErrorListConverter))]
    public List<ApiError> Errors { get; set; } = [];
    public bool IsSuccess { get; set; }
}

public sealed class ApiError
{
    public int? Code { get; set; }
    public string Message { get; set; } = string.Empty;
}
