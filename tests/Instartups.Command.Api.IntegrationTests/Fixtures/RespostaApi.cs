namespace Instartups.Command.Api.IntegrationTests.Fixtures;

/// <summary>
/// Espelho do ResponseDTO&lt;T&gt; da Api, que não desserializa por ter setters private init.
/// </summary>
public sealed record RespostaApi<T>(
    bool IsSuccess,
    string Message,
    DateTimeOffset DateTimeResponse,
    T? Data
);

public sealed record IdResposta(Guid Id);

public sealed record ErroValidacao(string Campo, string Codigo, string Mensagem);
