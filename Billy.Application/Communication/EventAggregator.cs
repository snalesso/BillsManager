using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Communication
{
    /// <inheritdoc />
    public class EventAggregator : IEventAggregator
    {
        private readonly List<Handler> _handlers = new List<Handler>();

        /// <inheritdoc />
        public virtual bool HandlerExistsFor(Type messageType)
        {
            lock (this._handlers)
            {
                return this._handlers.Any(handler => handler.Handles(messageType) && !handler.IsDead);
            }
        }

        /// <inheritdoc />
        public virtual void Subscribe(object subscriber, Func<Func<Task>, Task> marshal)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            if (marshal == null)
            {
                throw new ArgumentNullException(nameof(marshal));
            }

            lock (this._handlers)
            {
                if (this._handlers.Any(x => x.Matches(subscriber)))
                {
                    return;
                }

                this._handlers.Add(new Handler(subscriber, marshal));
            }
        }

        /// <inheritdoc />
        public virtual void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            lock (this._handlers)
            {
                var found = this._handlers.FirstOrDefault(x => x.Matches(subscriber));

                if (found != null)
                {
                    this._handlers.Remove(found);
                }
            }
        }

        /// <inheritdoc />
        public virtual Task PublishAsync(object message, Func<Func<Task>, Task> marshal, CancellationToken cancellationToken = default)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (marshal == null)
            {
                throw new ArgumentNullException(nameof(marshal));
            }

            Handler[] toNotify;

            lock (this._handlers)
            {
                toNotify = this._handlers.ToArray();
            }

            return marshal(async () =>
            {
                var messageType = message.GetType();

                var tasks = toNotify.Select(h => h.Handle(messageType, message, CancellationToken.None));

                await Task.WhenAll(tasks);

                var deadHandlers = toNotify.Where(h => h.IsDead).ToList();

                if (deadHandlers.Any())
                {
                    lock (this._handlers)
                    {
                        foreach (var handler in deadHandlers)
                        {
                            this._handlers.Remove(handler);
                        }
                    }
                }
            });
        }

        private class Handler
        {
            private readonly Func<Func<Task>, Task> _marshal;
            private readonly WeakReference _reference;
            private readonly Dictionary<Type, MethodInfo> _supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler, Func<Func<Task>, Task> marshal)
            {
                this._marshal = marshal;
                this._reference = new WeakReference(handler);

                //var interfaces = handler.GetType().GetTypeInfo().ImplementedInterfaces
                //    .Where(x => typeof(IHandle).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo()) && x.GetTypeInfo().IsGenericType);

                var interfaces = handler.GetType().GetTypeInfo().ImplementedInterfaces
                    .Where(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandle<>));

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetTypeInfo().GenericTypeArguments[0];
                    var method = @interface.GetRuntimeMethod(nameof(IHandle<dynamic>.HandleAsync), new[] { type, typeof(CancellationToken) });

                    if (method != null)
                    {
                        this._supportedHandlers[type] = method;
                    }
                }
            }

            public bool IsDead => this._reference.Target == null;

            public bool Matches(object instance)
            {
                return this._reference.Target == instance;
            }

            public Task Handle(Type messageType, object message, CancellationToken cancellationToken)
            {
                var target = this._reference.Target;

                if (target == null)
                {
                    return Task.FromResult(false);
                }

                return this._marshal(() =>
                {
                    var tasks = this._supportedHandlers
                        .Where(handler => handler.Key.GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo()))
                        .Select(pair => pair.Value.Invoke(target, new[] { message, cancellationToken }))
                        .Select(result => (Task)result)
                        .ToList();

                    return Task.WhenAll(tasks);
                });
            }

            public bool Handles(Type messageType)
            {
                return this._supportedHandlers.Any(pair => pair.Key.GetTypeInfo().IsAssignableFrom(messageType.GetTypeInfo()));
            }
        }
    }
}