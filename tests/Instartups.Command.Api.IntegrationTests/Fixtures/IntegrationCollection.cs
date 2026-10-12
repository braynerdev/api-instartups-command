namespace Instartups.Command.Api.IntegrationTests.Fixtures;

/// <summary>
/// Um único host e um único banco para todos os testes; a collection serializa a execução,
/// já que cada teste reseta o banco antes de rodar.
/// </summary>
[CollectionDefinition(Nome)]
public sealed class IntegrationCollection : ICollectionFixture<InstartupsApiFactory>
{
    public const string Nome = "Integracao";
}
