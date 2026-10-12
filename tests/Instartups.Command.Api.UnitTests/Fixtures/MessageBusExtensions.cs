using Wolverine;

namespace Instartups.Command.Api.UnitTests.Fixtures;

public static class MessageBusExtensions
{
    public static TCommand UnicoComandoEnviado<TCommand>(this IMessageBus messageBus, CancellationToken ctEsperado)
    {
        var chamada = messageBus.ReceivedCalls().ShouldHaveSingleItem();
        chamada.GetMethodInfo().Name.ShouldBe(nameof(IMessageBus.InvokeAsync));

        var argumentos = chamada.GetArguments();
        argumentos[1].ShouldBe(ctEsperado);

        return argumentos[0].ShouldBeOfType<TCommand>();
    }
}
