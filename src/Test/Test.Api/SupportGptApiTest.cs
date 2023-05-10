using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Moq;
using PrimeFuncPack;

namespace GarageGroup.Internal.Support.GptApi.Test;

public static partial class SupportGptApiTest
{
    private const string SomeResponseMessage = "Some response message";

    private static readonly SupportGptApiOption SomeOption
        =
        new(
            apiKey: "Some API key",
            incidentComplete: new(
                model: "some-gpt-model")
            {
                ChatMessages = new ChatMessageOption("some-role", "Some message: {0}").AsFlatArray(),
                MaxTokens = 150,
                Temperature = 0.7m
            });

    private static readonly IncidentCompleteIn SomeInput
        =
        new("Some customer message");

    private static HttpResponseMessage CreateSuccessResponse(string message)
    {
        var outJson = new StubGptJsonOut
        {
            Choices = new StubChoiceJson[]
            {
                new()
                {
                    Message = new()
                    {
                        Content = message
                    },
                    FinishReason = "stop"
                }
            }
        };

        var json = JsonSerializer.Serialize(outJson);

        return new(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };
    }

    private static ISupportGptApi CreateSupportGptApi(HttpMessageHandler httpMessageHandler, SupportGptApiOption option)
        =>
        Dependency.Of(httpMessageHandler, option).UseSupportGptApi().Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IAsyncFunc<HttpRequestMessage, HttpResponseMessage>> CreateMockProxyHandler(
        HttpResponseMessage responseMessage, Action<HttpRequestMessage>? callback = default)
    {
        var mock = new Mock<IAsyncFunc<HttpRequestMessage, HttpResponseMessage>>();

        var m = mock
            .Setup(p => p.InvokeAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseMessage);

        if (callback is not null)
        {
            _ = m.Callback<HttpRequestMessage, CancellationToken>(
                (r, _) => callback.Invoke(r));
        }

        return mock;
    }
}