using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class SupportGptApi
{
    public ValueTask<Result<IncidentCompleteOut, Failure<Unit>>> CompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<Result<IncidentCompleteOut, Failure<Unit>>>(cancellationToken);
        }

        return InnerCompleteIncidentAsync(input, cancellationToken);
    }

    private async ValueTask<Result<IncidentCompleteOut, Failure<Unit>>> InnerCompleteIncidentAsync(
        IncidentCompleteIn input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(input.Message))
        {
            return default(IncidentCompleteOut);
        }

        var jsonIn = new ChatGptJsonIn
        {
            Model = option.IncidentComplete.Model,
            MaxTokens = option.IncidentComplete.MaxTokens,
            Temperature = option.IncidentComplete.Temperature,
            Top = 1,
            Messages = option.IncidentComplete.ChatMessages.Map(CreateChatMessageJson)
        };

        var json = JsonSerializer.Serialize(jsonIn);
        using var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var httpClient = CreateHttpClient();

        using var httpResponse = await httpClient.PostAsync(OpenAiCompletionsUrl, content, cancellationToken).ConfigureAwait(false);
        var httpResponseText = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (httpResponse.IsSuccessStatusCode is false)
        {
            return Failure.Create($"An unexpected http status code: {httpResponse.StatusCode}. Body: {httpResponseText}");
        }

        var jsonOut = JsonSerializer.Deserialize<ChatGptJsonOut>(httpResponseText);
        var choice = jsonOut.Choices.FirstOrAbsent().OrDefault();

        if (choice is null)
        {
            return Failure.Create($"GPT result choices are absent. Source message: {input.Message}");
        }

        if (string.Equals(choice.FinishReason, "stop", StringComparison.InvariantCultureIgnoreCase) is false)
        {
            return Failure.Create($"An unexpected GPT finish reason: {choice.FinishReason}. Source message: {input.Message}");
        }

        return new IncidentCompleteOut
        {
            Title = choice.Message?.Content?.Trim()?.TrimEnd('.')
        };

        ChatMessageJson CreateChatMessageJson(ChatMessageOption messageOption)
            =>
            new()
            {
                Role = messageOption.Role,
                Content = string.Format(messageOption.ContentTemplate, input.Message)
            };
    }
}