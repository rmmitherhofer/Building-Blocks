using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Enums;
using Zypher.Http;
using Zypher.Http.Exceptions;
using Zypher.Http.Extensions;
using Zypher.Json;
using Zypher.Notifications.Interfaces;
using Zypher.Notifications.Messages;
using Zypher.Responses;

namespace Zypher.Http.Tests.Services;

public class HttpServiceTests
{
    private sealed class TestNotificationHandler : INotificationHandler
    {
        private readonly List<Notification> _notifications = new();

        public void Notify(Notification notification) => _notifications.Add(notification);

        public void Notify(IEnumerable<Notification> notifications) => _notifications.AddRange(notifications);

        public IEnumerable<Notification> Get() => _notifications;

        public bool HasNotifications() => _notifications.Count > 0;

        public void Clear() => _notifications.Clear();
    }

    private sealed class TestLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable BeginScope<TState>(TState state) => new DummyScope();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }

        private sealed class DummyScope : IDisposable
        {
            public void Dispose() { }
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public HttpRequestMessage? LastRequest { get; private set; }

        public List<HttpMethod> Methods { get; } = new();

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            Methods.Add(request.Method);
            var response = _handler(request);
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }

    private sealed class TestHttpService : HttpService
    {
        public TestHttpService(HttpClient httpClient, INotificationHandler notification, ILogger logger)
            : base(httpClient, notification, logger) { }

        public Task<HttpResponseMessage> GetAsyncPublic((string template, string uri) uri) => GetAsync(uri);
        public Task<HttpResponseMessage> GetAsyncPublic(string uri) => GetAsync(uri);
        public HttpResponseMessage GetPublic(string uri) => Get(uri);
        public Task<HttpResponseMessage> PostAsyncPublic(string uri, HttpContent content) => PostAsync(uri, content);
        public HttpResponseMessage PostPublic(string uri, HttpContent content) => Post(uri, content);
        public Task<HttpResponseMessage> PutAsyncPublic(string uri, HttpContent content) => PutAsync(uri, content);
        public HttpResponseMessage PutPublic(string uri, HttpContent content) => Put(uri, content);
        public Task<HttpResponseMessage> PatchAsyncPublic(string uri, HttpContent content) => PatchAsync(uri, content);
        public HttpResponseMessage PatchPublic(string uri, HttpContent content) => Patch(uri, content);
        public Task<HttpResponseMessage> DeleteAsyncPublic(string uri) => DeleteAsync(uri);
        public HttpResponseMessage DeletePublic(string uri) => Delete(uri);
        public void EnableDetailedLoggingPublic() => EnableLogHeadersAndBody();

        public Task<TResponse?> ValidateAndReturnPublic<TResponse>(HttpResponseMessage response)
            => ValidateAndReturn<TResponse>(response);

        public Task ValidateResponsePublic(HttpResponseMessage response, bool throwException = false)
            => ValidateResponse(response, throwException);
    }

    private sealed class SamplePayload
    {
        public string? Name { get; set; }
    }

    [Fact(DisplayName =
        "Given a response with JSON content, " +
        "When ValidateAndReturn is called, " +
        "Then it deserializes and returns the payload")]
    [Trait("Type", nameof(HttpService))]
    public async Task ValidateAndReturn_DeserializesPayload()
    {
        //Given
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var service = new TestHttpService(client, new TestNotificationHandler(), new TestLogger());

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"name\":\"John\"}")
        };

        //When
        var result = await service.ValidateAndReturnPublic<SamplePayload>(response);

        //Then
        result.Should().NotBeNull();
        result!.Name.Should().Be("John");
    }

    [Fact(DisplayName =
        "Given a bad request response with issues, " +
        "When ValidateResponse is called, " +
        "Then it notifies all issue details")]
    [Trait("Type", nameof(HttpService))]
    public async Task ValidateResponse_BadRequest_NotifiesIssues()
    {
        //Given
        var notifications = new TestNotificationHandler();
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var service = new TestHttpService(client, notifications, new TestLogger());

        var apiResponse = new ApiResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Issues = new List<IssuerResponse>
            {
                new(IssuerResponseType.Validation)
                {
                    Details =
                    [
                        new NotificationResponse
                        {
                            LogLevel = LogLevel.Warning,
                            Type = "Validation",
                            Key = "field",
                            Value = "required",
                            Detail = "missing"
                        }
                    ]
                }
            }
        };

        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(JsonExtensions.Serialize(apiResponse))
        };

        //When
        await service.ValidateResponsePublic(response);

        //Then
        notifications.HasNotifications().Should().BeTrue();
        notifications.Get().Should().ContainSingle();
    }

    [Fact(DisplayName =
        "Given an unauthorized response, " +
        "When ValidateResponse is called, " +
        "Then it throws CustomHttpRequestException")]
    [Trait("Type", nameof(HttpService))]
    public async Task ValidateResponse_Unauthorized_Throws()
    {
        //Given
        var notifications = new TestNotificationHandler();
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var service = new TestHttpService(client, notifications, new TestLogger());

        var apiResponse = new ApiResponse
        {
            StatusCode = HttpStatusCode.Unauthorized,
            Issues = new List<IssuerResponse>()
        };

        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(JsonExtensions.Serialize(apiResponse)),
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com/users")
        };

        //When
        Func<Task> action = () => service.ValidateResponsePublic(response);

        //Then
        await action.Should().ThrowAsync<CustomHttpRequestException>();
    }

    [Fact(DisplayName =
        "Given a response with errors but no issues, " +
        "When ValidateResponse is called, " +
        "Then it does not notify and does not throw for allowed status")]
    [Trait("Type", nameof(HttpService))]
    public async Task ValidateResponse_ErrorsWithoutIssues_DoesNotNotify()
    {
        //Given
        var notifications = new TestNotificationHandler();
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var service = new TestHttpService(client, notifications, new TestLogger());

        var apiResponse = new ApiResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            Issues = new List<IssuerResponse>()
        };

        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(JsonExtensions.Serialize(apiResponse))
        };

        //When
        await service.ValidateResponsePublic(response);

        //Then
        notifications.HasNotifications().Should().BeFalse();
    }

    [Fact(DisplayName =
        "Given a response with an unhandled status code, " +
        "When ValidateResponse is called, " +
        "Then it throws CustomHttpRequestException")]
    [Trait("Type", nameof(HttpService))]
    public async Task ValidateResponse_UnhandledStatus_Throws()
    {
        //Given
        var notifications = new TestNotificationHandler();
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage((HttpStatusCode)418));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };
        var service = new TestHttpService(client, notifications, new TestLogger());

        var apiResponse = new ApiResponse
        {
            StatusCode = (HttpStatusCode)418,
            Issues = new List<IssuerResponse>()
        };

        var response = new HttpResponseMessage((HttpStatusCode)418)
        {
            Content = new StringContent(JsonExtensions.Serialize(apiResponse)),
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com/users")
        };

        //When
        Func<Task> action = () => service.ValidateResponsePublic(response);

        //Then
        await action.Should().ThrowAsync<CustomHttpRequestException>();
    }

    [Fact(DisplayName =
        "Given all http verbs, " +
        "When they are called, " +
        "Then the client sends each expected method")]
    [Trait("Type", nameof(HttpService))]
    public async Task HttpService_Verbs_SendExpectedMethods()
    {
        //Given
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
        var service = new TestHttpService(client, new TestNotificationHandler(), new TestLogger());
        var content = new StringContent("{\"name\":\"John\"}");

        //When
        await service.GetAsyncPublic("users");
        service.GetPublic("users");
        await service.PostAsyncPublic("users", content);
        service.PostPublic("users", content);
        await service.PutAsyncPublic("users/1", content);
        service.PutPublic("users/1", content);
        await service.PatchAsyncPublic("users/1", content);
        service.PatchPublic("users/1", content);
        await service.DeleteAsyncPublic("users/1");
        service.DeletePublic("users/1");

        //Then
        handler.Methods.Should().BeEquivalentTo(new[]
        {
            HttpMethod.Get,
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Patch,
            HttpMethod.Delete,
            HttpMethod.Delete
        }, options => options.WithStrictOrdering());
    }

    [Fact(DisplayName =
        "Given a request template, " +
        "When GetAsync is called with template tuple, " +
        "Then logs include the template uri")]
    [Trait("Type", nameof(HttpService))]
    public async Task GetAsync_WithTemplate_LogsTemplateUri()
    {
        //Given
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var logger = new TestLogger();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
        var service = new TestHttpService(client, new TestNotificationHandler(), logger);

        //When
        await service.GetAsyncPublic(("/users/{id}", "users/1"));

        //Then
        logger.Entries.Should().Contain(e => e.Message.Contains("https://example.com//users/{id}", StringComparison.Ordinal));
    }

    [Fact(DisplayName =
        "Given detailed logging enabled, " +
        "When PostAsync is called, " +
        "Then it logs headers and content")]
    [Trait("Type", nameof(HttpService))]
    public async Task PostAsync_DetailedLogging_LogsHeadersAndContent()
    {
        //Given
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var logger = new TestLogger();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/") };
        var service = new TestHttpService(client, new TestNotificationHandler(), logger);
        service.EnableDetailedLoggingPublic();
        var content = new StringContent("{\"name\":\"John\"}");

        //When
        await service.PostAsyncPublic("users", content);

        //Then
        logger.Entries.Should().Contain(e => e.Message.Contains("|Headers:", StringComparison.Ordinal));
        logger.Entries.Should().Contain(e => e.Message.Contains("|Content:", StringComparison.Ordinal));
    }
}
