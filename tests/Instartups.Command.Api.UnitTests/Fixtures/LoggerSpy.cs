using Microsoft.Extensions.Logging;

namespace Instartups.Command.Api.UnitTests.Fixtures;

public sealed class LoggerSpy<T> : ILogger<T>
{
    public sealed record Registro(LogLevel Nivel, string Mensagem, Exception? Excecao);

    private readonly List<Registro> _registros = [];
    public IReadOnlyList<Registro> Registros => _registros;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _registros.Add(new Registro(logLevel, formatter(state, exception), exception));
    }
}
