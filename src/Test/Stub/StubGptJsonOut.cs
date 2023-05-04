using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support.GptApi.Test;

internal sealed record class StubGptJsonOut
{
    [JsonPropertyName("choices")]
    public StubChoiceJson[]? Choices { get; init; }
}