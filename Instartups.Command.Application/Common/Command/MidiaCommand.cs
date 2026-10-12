using Instartups.Command.Domain.Enums;

namespace Instartups.Command.Application.Common.Command;

public sealed record MidiaCommand(
    string Url,
    TiposMidiaEnum Tipo);