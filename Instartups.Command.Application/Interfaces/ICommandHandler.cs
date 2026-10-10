using System;
using System.Collections.Generic;
using System.Text;

namespace Instartups.Command.Application.Interfaces;

public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand
{
    public Task<TResponse> Handle(TCommand Command, CancellationToken ct);
}

public interface ICommandHandler<TResponse>
{
    public Task<TResponse> Handle(CancellationToken ct);
}

public interface IVoidCommandHandler<TCommand> where TCommand : ICommand
{
    public Task Handle(TCommand command, CancellationToken ct);
}
