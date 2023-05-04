using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal sealed record class ChatFailureJson
{
    [JsonPropertyName("error")]
    public ChatErrorInfoJson? Error { get; init; }
}