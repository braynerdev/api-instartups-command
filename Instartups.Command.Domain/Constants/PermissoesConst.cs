using System.Reflection;


namespace Instartups.Command.Domain.Constants;

public static class PermissoesConst
{
    public const string Admin = "Admin";
    
    public static class Perfil
    {
        public const string Criar = "CRIAR.PERFIL";
        public const string Ler = "LER.PERFIL";
        public const string Editar = "EDITAR.PERFIL";
        public const string Desativar = "DESATIVAR.PERFIL";
        public const string Ativar = "ATIVAR.PERFIL";
        public const string Seguir = "SEGUIR.PERFIL";
    }

    public static class Postagem
    {
        public const string Criar = "CRIAR.POSTAGEM";
        public const string Ler = "LER.POSTAGEM";
        public const string Editar = "EDITAR.POSTAGEM";
        public const string Desativar = "DESATIVAR.POSTAGEM";
        public const string Ativar = "ATIVAR.POSTAGEM";
        public const string Curtir = "CURTIR.POSTAGEM";
    }
    
    private static readonly IReadOnlyCollection<string> _all =
        typeof(PermissoesConst)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string) && f.Name != nameof(Admin))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToArray();

    public static IReadOnlyCollection<string> All() => _all;
    
}