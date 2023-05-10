using System.Text.Json.Serialization;

namespace GarageGroup.Internal.Support.GptApi.Test;

internal sealed record class StubFailureJson
{
    [JsonPropertyName("error")]
    public StubErrorInfoJson? Error { get; init; }
}