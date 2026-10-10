namespace Instartups.Command.Domain.Constants;

public static class MensagensErroConst
{
    public const string FormatoInvalido = "Formato inválido.";
    public const string CampoObrigatorio = "O campo {PropertyName} é obrigatório.";
    public const string ValorEntre = "O valor do campo {PropertyName} deve estar entre {From} e {To}.";
    public const string TamanhoMaximo = "O tamanho do campo {PropertyName} deve ser no máximo {MaxLength}.";
    public const string UrlInvalida = "O campo {PropertyName} deve ser uma URL válida.";
    public const string ValorMinimo = "O valor do campo {PropertyName} deve ser maior que {ComparisonValue}.";
    public const string PrecisaoIncorreta = "O valor do campo {PropertyName} excede a precisão ou escala permitida.";
}
