using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class ChatErrorInfoJson
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }
}