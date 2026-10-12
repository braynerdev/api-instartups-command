using Instartups.Command.Domain.Constants;

namespace Instartups.Command.Api.IntegrationTests.Fixtures;

public static class Rotas
{
    public static class Perfil
    {
        public const string Investidor = $"{RotasBaseConst.Perfil}/investidor";
        public const string Startup = $"{RotasBaseConst.Perfil}/startup";
        public const string Coordenada = $"{RotasBaseConst.Perfil}/coordenada";
        public const string Desativar = $"{RotasBaseConst.Perfil}/desativar";
        public const string Reativar = $"{RotasBaseConst.Perfil}/reativar";
        public static string Seguir(Guid seguidoId) => $"{RotasBaseConst.Perfil}/{seguidoId}/seguir";
    }

    public static class Postagem
    {
        public const string Base = RotasBaseConst.Postagem;
        public static string PorId(Guid postagemId) => $"{RotasBaseConst.Postagem}/{postagemId}";
        public static string Desativar(Guid postagemId) => $"{RotasBaseConst.Postagem}/{postagemId}/desativar";
        public static string Reativar(Guid postagemId) => $"{RotasBaseConst.Postagem}/{postagemId}/reativar";
        public static string Curtir(Guid postagemId) => $"{RotasBaseConst.Postagem}/{postagemId}/curtir";
    }
}
