using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Domain.CQRS
{
    public interface IMessage { }
    public interface IHandler<in T> where T : IMessage
    {
        Task Handle(T message);
    }
    public interface ICancellableHandler<in TMessage> where TMessage : IMessage
    {
        Task Handle(TMessage message, CancellationToken token = default);
    }

    namespace Query
    {
        public interface IQuery<TReturn> : IMessage { }
        public interface ICancellableQueryHandler<in TMessage, TReturn> where TMessage : IQuery<TReturn>
        {
            Task<TReturn> Handle(TMessage message, CancellationToken token = default);
        }
        public interface IQueryHandler<in TMessage, TResponse> where TMessage : IQuery<TResponse>
        {
            Task<TResponse> HandleAsync(TMessage query);
        }
        public interface IQueryProcessor
        {
            Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
        }
    }

    namespace Command
    {
        public interface ICommand : IMessage { }
        public interface ICommandSender
        {
            Task Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand;
        }
        public interface ICommandHandler<in TCommand> : IHandler<TCommand> where TCommand : ICommand
        {
        }
        public interface ICancellableCommandHandler<in TCommand> : ICancellableHandler<TCommand> where TCommand : ICommand
        {
        }
    }

    namespace Routing
    {
        public interface IHandlerRegistrar
        {
            void RegisterHandler<TMessage>(Func<TMessage, CancellationToken, Task> handler) where TMessage : class, IMessage;
        }
    }
}
