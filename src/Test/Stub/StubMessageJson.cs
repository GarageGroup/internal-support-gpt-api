using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support.GptApi.Test;

internal sealed record class StubMessageJson
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }
}