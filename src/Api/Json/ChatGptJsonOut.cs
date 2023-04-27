using System;
using System.Text.Json.Serialization;

namespace GGroupp.Internal.Support;

internal readonly record struct ChatGptJsonOut
{
    [JsonPropertyName("choices")]
    public FlatArray<ChatChoiceJson> Choices { get; init; }
}