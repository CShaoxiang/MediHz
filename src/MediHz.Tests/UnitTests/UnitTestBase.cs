using AutoFixture;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public abstract class UnitTestBase : IAsyncLifetime
{
    private readonly ILoggerFactory _loggerFactory;

    protected UnitTestBase(ITestOutputHelper output)
    {
        // Handles logging only when tests fail.
        _loggerFactory = LoggerFactory.Create(b => b.AddXUnit(output));
    }

    protected ILogger<T> Logger<T>() => _loggerFactory.CreateLogger<T>();

    protected readonly Fixture Fixture = new();

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        _loggerFactory.Dispose();
        await Task.CompletedTask;
    }
}
