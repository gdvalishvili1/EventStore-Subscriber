using EventStore.Subscriber.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EventStore.Subscriber.Dispatcher
{
    public class DefaultEventDispatcher : IStoredEventDispatcher
    {
        private readonly IDictionary<Type, List<Type>> _eventTypeHandlers =
            new Dictionary<Type, List<Type>>();
        private readonly IServiceProvider _serviceProvider;

        public DefaultEventDispatcher(IServiceProvider serviceProvider, Assembly eventsAssembly,
            params Assembly[] eventHandlersAssemblies)
        {
            _eventTypeHandlers = EventHandlerMapping.Mapping<IHandleStoredEvent<IStoredEvent>>(eventsAssembly, eventHandlersAssemblies);
            _serviceProvider = serviceProvider;
        }

        public EventDispatchResult Dispatch(dynamic evnt)
        {
            try
            {
                var type = evnt.GetType();
                if (!_eventTypeHandlers.ContainsKey(type))
                {
                    return new EventDispatchResult
                    {
                        Success = true
                    };
                }

                var @eventHandlers = _eventTypeHandlers[type];
                foreach (var handlr in @eventHandlers)
                {
                    var eventHandler = _serviceProvider?.GetService(handlr);
                    if (eventHandler == null)
                    {
                        eventHandler = Activator.CreateInstance(handlr);
                    }

                    var handlerInstance = eventHandler as dynamic;
                    handlerInstance.Handle(evnt, new StoredEventDispatchContext(_serviceProvider));
                }

                return new EventDispatchResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new EventDispatchResult
                {
                    Success = false,
                    Error = ex.ToString()
                };
            }
        }
    }
}
