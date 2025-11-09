using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using API.Hubs;

namespace API.Logging;
public class SignalRLoggerProvider : ILoggerProvider
{
    private readonly IServiceProvider _services;
    private bool _disposed;

    public SignalRLoggerProvider(IServiceProvider services)
    {
        _services = services;
    }

    public ILogger CreateLogger(string categoryName) => new SignalRLogger(_services, categoryName);

    public void Dispose() => _disposed = true;

    private class SignalRLogger : ILogger
    {
        private readonly IServiceProvider _services;
        private readonly string _category;

        public SignalRLogger(IServiceProvider services, string category)
        {
            _services = services;
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                var message = formatter(state, exception);
                var text = $"{DateTime.UtcNow:O} [{logLevel}] {_category} - {message}";
                if (exception != null) text += $" | Exception: {exception}";

                var hubContext = _services.GetService<IHubContext<LogHub>>();
                if (hubContext != null)
                {
                    _ = hubContext.Clients.All.SendAsync("ReceiveLog", text);
                }
            }
            catch
            {
                // Não propagar falhas de logging
            }
        }
    }
}