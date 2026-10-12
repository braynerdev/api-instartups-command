using System.Reflection;

namespace Instartups.Command.Tests.Common.Extensions;

public static class ReflectionExtensions
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    public static T DefinirPropriedadePrivada<T>(this T objeto, string nomePropriedade, object? valor)
        where T : notnull
    {
        var tipo = objeto.GetType();

        while (tipo is not null)
        {
            var propriedade = tipo.GetProperty(nomePropriedade, Flags);

            if (propriedade?.SetMethod is not null)
            {
                propriedade.SetValue(objeto, valor);
                return objeto;
            }

            tipo = tipo.BaseType;
        }

        throw new InvalidOperationException($"Propriedade '{nomePropriedade}' com setter não encontrada em '{objeto.GetType().Name}'.");
    }
}
