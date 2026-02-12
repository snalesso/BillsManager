using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Communication
{
    public interface IBusEvent
    {
        object Data { get; }
    }

    public interface IBusEvent<T> : IBusEvent
    {
        new T Data { get; }
    }

    public class BusEvent<T> : IBusEvent<T>
    {
        public BusEvent(T data)
        {
            this.Data = data;
        }

        public T Data { get; }

        object IBusEvent.Data => this.Data;
    }

    public interface IBusEventHandler
    {
        Task Handle(IBusEvent e);
    }

    public interface IBusEventHandler<T> : IBusEventHandler
    {
        Task Handle(IBusEvent<T> e);
    }

    public interface IBus
    {
        Task Publish<T>(BusEvent<T> e);

        IBus RegisterHandler<T>(IBusEventHandler<T> handler);
    }

    public delegate Task HandleBusEvent<T>(BusEvent<T> e);
    public delegate Task HandleBusEvent(IBusEvent e);

    public class SimpleBus : IBus
    {
        private IDictionary<Type, IList<
            //IBusEventHandler<Type>
            HandleBusEvent
            >> _handlers;

        public Task Publish<T>(BusEvent<T> e)
        {
            if (e == null)
            {
                throw new ArgumentNullException();
            }

            if (this._handlers.TryGetValue(typeof(T), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler.DynamicInvoke(e);
                }
            }

            return Task.CompletedTask;
        }

        public IBus RegisterHandler<T>(IBusEventHandler<T> handler)
        {
            if (this._handlers.TryGetValue(typeof(T), out var tHandlers) == false)
            {
                if (tHandlers == null)
                {
                    var handlerDelegate =
                        //Delegate.CreateDelegate(typeof(HandleBusEvent), handler, nameof(IBusEventHandler<T>.Handle));
                        new HandleBusEvent(handler.Handle);

                    tHandlers = new List<HandleBusEvent>
                    {
                        handlerDelegate
                    };

                    this._handlers.TryAdd(typeof(T), tHandlers);
                }
            }

            return this;
        }
    }
}
