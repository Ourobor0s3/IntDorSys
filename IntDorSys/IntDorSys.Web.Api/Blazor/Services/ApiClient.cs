using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IntDorSys.Web.Api.Blazor.Models;

namespace IntDorSys.Web.Api.Blazor.Services;

public sealed class ApiClient(HttpClient httpClient, AuthSession auth)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<ApiResponse<AuthToken>?> LoginAsync(string email, string password, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("token", new LoginRequest
            {
                Login = email,
                Password = password,
            }, ct);

            var result = await ReadApiResponseAsync<AuthToken>(response, ct);

            if (result.Data?.AccessToken is { Length: > 0 } token)
            {
                var role = result.Data.Role;
                await auth.LoginAsync(token, role);
            }

            return result;
        }
        catch (Exception ex)
        {
            return CreateError<AuthToken>($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>?> RegisterAsync(string email, string fullName, string numGroup, string numRoom, string password, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("register", new RegisterRequest
            {
                Email = email,
                FullName = fullName,
                NumGroup = numGroup,
                NumRoom = numRoom,
                Password = password,
            }, ct);

            return await ReadApiResponseAsync<bool>(response, ct);
        }
        catch (Exception ex)
        {
            return CreateError<bool>($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<UserInfoViewModel>>?> GetUsersAsync(CancellationToken ct)
    {
        using var request = CreateAuthorized(HttpMethod.Get, "users-info");
        return await SendAsync<List<UserInfoViewModel>>(request, ct);
    }

    public async Task<ApiResponse<bool>?> ChangeUserStatusAsync(long userId, int status, CancellationToken ct)
    {
        using var request = CreateAuthorized(HttpMethod.Put, $"users-info/change-status/{userId}");
        request.Content = JsonContent.Create(status);
        return await SendAsync<bool>(request, ct);
    }

    public async Task<ApiResponse<List<LaundressPageViewModel>>?> GetLaundressAsync(DateTime? startDate, DateTime? endDate, CancellationToken ct)
    {
        var path = "laund";
        var query = new List<string>();
        if (startDate.HasValue)
        {
            query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}");
        }
        if (endDate.HasValue)
        {
            query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}");
        }
        if (query.Count > 0)
        {
            path += "?" + string.Join("&", query);
        }

        using var request = CreateAuthorized(HttpMethod.Get, path);
        return await SendAsync<List<LaundressPageViewModel>>(request, ct);
    }

    public async Task<ApiResponse<List<ReportViewModel>>?> GetReportsAsync(DateTime? startDate, DateTime? endDate, CancellationToken ct)
    {
        var path = "laund/reports";
        var query = new List<string>();
        if (startDate.HasValue)
        {
            query.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("o"))}");
        }

        if (endDate.HasValue)
        {
            query.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("o"))}");
        }

        if (query.Count > 0)
        {
            path += "?" + string.Join("&", query);
        }

        using var request = CreateAuthorized(HttpMethod.Get, path);
        return await SendAsync<List<ReportViewModel>>(request, ct);
    }

    public async Task<ApiResponse<List<ChartPoint>>?> GetAnalyticsAsync(CancellationToken ct)
    {
        using var request = CreateAuthorized(HttpMethod.Get, "analitic/laund");
        return await SendAsync<List<ChartPoint>>(request, ct);
    }

    private HttpRequestMessage CreateAuthorized(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);
        if (auth.IsLoggedIn)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        }

        return request;
    }

    private async Task<ApiResponse<T>?> SendAsync<T>(HttpRequestMessage request, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.SendAsync(request, ct);
            return await ReadApiResponseAsync<T>(response, ct);
        }
        catch (Exception ex)
        {
            return CreateError<T>($"Request failed: {ex.Message}");
        }
    }

    private static ApiResponse<T> CreateError<T>(string message)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Errors = [new ApiError { Message = message }],
        };
    }

    private static async Task<ApiResponse<T>> ReadApiResponseAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var text = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(text))
        {
            return CreateError<T>($"Empty response, status {(int)response.StatusCode}");
        }

        try
        {
            using var document = JsonDocument.Parse(text);
            var root = document.RootElement;

            var result = new ApiResponse<T>
            {
                IsSuccess = root.TryGetProperty("isSuccess", out var successElement) && successElement.ValueKind == JsonValueKind.True,
                Errors = ReadErrorsFromRoot(root),
            };

            if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind != JsonValueKind.Null)
            {
                result.Data = dataElement.Deserialize<T>(JsonOptions);
            }

            if (!response.IsSuccessStatusCode && result.Errors.Count == 0)
            {
                result.Errors.Add(new ApiError { Message = $"HTTP {(int)response.StatusCode}" });
            }

            return result;
        }
        catch
        {
            return CreateError<T>(text);
        }
    }

    private static List<ApiError> ReadErrorsFromRoot(JsonElement root)
    {
        if (!root.TryGetProperty("errors", out var errorsElement) || errorsElement.ValueKind == JsonValueKind.Null)
        {
            return [];
        }

        if (errorsElement.ValueKind == JsonValueKind.Array)
        {
            var list = new List<ApiError>();
            foreach (var element in errorsElement.EnumerateArray())
            {
                list.AddRange(ReadErrors(element));
            }

            return list;
        }

        if (errorsElement.ValueKind == JsonValueKind.Object)
        {
            return ReadErrors(errorsElement);
        }

        if (errorsElement.ValueKind == JsonValueKind.String)
        {
            return [new ApiError { Message = errorsElement.GetString() ?? "Unknown error" }];
        }

        return [new ApiError { Message = errorsElement.ToString() }];
    }

    private static List<ApiError> ReadErrors(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty("message", out var messageElement))
            {
                return [new ApiError
                {
                    Message = messageElement.GetString() ?? "Unknown error",
                    Code = element.TryGetProperty("code", out var codeElement) && codeElement.TryGetInt32(out var code)
                        ? code
                        : null,
                }];
            }

            var list = new List<ApiError>();
            foreach (var property in element.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        list.Add(new ApiError { Message = item.ToString() });
                    }
                }
                else
                {
                    list.Add(new ApiError { Message = property.Value.ToString() });
                }
            }

            return list;
        }

        if (element.ValueKind == JsonValueKind.String)
        {
            return [new ApiError { Message = element.GetString() ?? "Unknown error" }];
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var list = new List<ApiError>();
            foreach (var item in element.EnumerateArray())
            {
                list.AddRange(ReadErrors(item));
            }

            return list;
        }

        return [new ApiError { Message = element.ToString() }];
    }
}
