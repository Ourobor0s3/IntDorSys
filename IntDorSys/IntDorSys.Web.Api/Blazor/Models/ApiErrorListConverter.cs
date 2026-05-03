using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntDorSys.Web.Api.Blazor.Models;

public sealed class ApiErrorListConverter : JsonConverter<List<ApiError>>
{
    public override List<ApiError> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return [];
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = JsonSerializer.Deserialize<List<ApiError>>(ref reader, options);
            return list ?? [];
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var single = JsonSerializer.Deserialize<ApiError>(ref reader, options);
            return single == null ? [] : [single];
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var message = reader.GetString() ?? "Unknown error";
            return [new ApiError { Message = message }];
        }

        using var document = JsonDocument.ParseValue(ref reader);
        return [new ApiError { Message = document.RootElement.ToString() }];
    }

    public override void Write(Utf8JsonWriter writer, List<ApiError> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
