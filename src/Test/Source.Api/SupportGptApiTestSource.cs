using System.Text.Json;

namespace GGroupp.Internal.Support.GptApi.Test;

internal static partial class SupportGptApiTestSource
{
    private static string ToJson<T>(this T value)
        =>
        JsonSerializer.Serialize(value);
}