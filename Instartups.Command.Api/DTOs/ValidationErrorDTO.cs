namespace Instartups.Command.Api.DTOs;

public sealed record ValidationErrorDTO(string Campo, string Codigo, string Mensagem);