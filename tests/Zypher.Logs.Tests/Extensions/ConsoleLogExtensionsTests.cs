using System;
using System.IO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Extensions.Core;
using Zypher.Logs.Extensions;

namespace Zypher.Logs.Tests.Extensions;

[Collection("ConsoleTests")]
public class ConsoleLogExtensionsTests
{
    private sealed class TestLogger : ILogger
    {
        public string? LastMessage { get; private set; }
        public LogLevel? LastLevel { get; private set; }

        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) => new DummyScope();
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            LastLevel = logLevel;
            LastMessage = formatter(state, exception);
        }

        private sealed class DummyScope : IDisposable
        {
            public void Dispose() { }
        }
    }

    [Fact(DisplayName =
        "Given an empty message, " +
        "When LogInfo is called, " +
        "Then it does nothing")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogInfo_EmptyMessage_DoesNothing()
    {
        //Given
        var logger = new TestLogger();

        //When
        Action action = () => logger.LogInfo(string.Empty);

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a configured accessor with correlation id, " +
        "When LogInfo is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogInfo_WithAccessor_DoesNotThrow()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.CORRELATION_ID] = "corr-123";
        var accessor = new HttpContextAccessor { HttpContext = context };
        ConsoleLogExtensions.Configure(accessor);

        var logger = new TestLogger();

        //When
        Action action = () => logger.LogInfo("message");

        //Then
        action.Should().NotThrow();
        logger.LastMessage.Should().Contain("corr-123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When LogWarn is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogWarn_DoesNotThrow()
    {
        //Given
        var logger = new TestLogger();

        //When
        Action action = () => logger.LogWarn("warn");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When LogFail is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogFail_DoesNotThrow()
    {
        //Given
        var logger = new TestLogger();

        //When
        Action action = () => logger.LogFail("fail");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When LogCrit is called, " +
        "Then it does not throw")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogCrit_DoesNotThrow()
    {
        //Given
        var logger = new TestLogger();

        //When
        Action action = () => logger.LogCrit("crit");

        //Then
        action.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When LogDbug is called, " +
        "Then it logs a debug entry")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogDbug_LogsDebug()
    {
        //Given
        var logger = new TestLogger();

        //When
        logger.LogDbug("debug");

        //Then
        logger.LastLevel.Should().Be(LogLevel.Debug);
        logger.LastMessage.Should().Contain("|DBUG|");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When LogTrce is called, " +
        "Then it logs a trace entry")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task LogTrce_LogsTrace()
    {
        //Given
        var logger = new TestLogger();

        //When
        logger.LogTrce("trace");

        //Then
        logger.LastLevel.Should().Be(LogLevel.Trace);
        logger.LastMessage.Should().Contain("|TRCE|");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given static logging methods, " +
        "When LogInfo and LogWarn are called, " +
        "Then they write to console")]
    [Trait("Type", nameof(ConsoleLogExtensions))]
    public async Task StaticLogMethods_WriteToConsole()
    {
        //Given
        var original = Console.Out;
        var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            //When
            ConsoleLogExtensions.LogInfo("hello");
            ConsoleLogExtensions.LogWarn("warn");

            //Then
            var output = writer.ToString();
            output.Should().Contain("info");
            output.Should().Contain("INFO");
            output.Should().Contain("WARN");
        }
        finally
        {
            Console.SetOut(original);
        }

        await Task.CompletedTask;
    }
}
