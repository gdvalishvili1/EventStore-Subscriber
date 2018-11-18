using EventStore.Subscriber.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EventStore.Subscriber.Dispatcher
{
    public class EventHandlerMapping
    {
        public static IDictionary<Type, List<Type>> Mapping<THandler>(Assembly eventsAssembly, Assembly[] eventHandlersAssembly)
        {
            IDictionary<Type, List<Type>> result =
            new Dictionary<Type, List<Type>>();

            var eventTypes = eventsAssembly.GetTypes();

            var events = eventTypes
                .Where(at => typeof(IStoredEvent).IsAssignableFrom(at)
                 && at.IsClass && !at.IsAbstract && !at.IsInterface);

            foreach (var @event in events)
            {
                result[@event] = new List<Type>();
                foreach (var assemblyType in eventHandlersAssembly.SelectMany(x => x.GetTypes()))
                {
                    var eventHandlers = assemblyType.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(THandler).GetGenericTypeDefinition());

                    if (eventHandlers != null)
                    {
                        foreach (var eventHandler in eventHandlers)
                        {
                            var genericarguments = eventHandler.GetGenericArguments().FirstOrDefault(x => @event == x);
                            if (genericarguments != null)
                            {
                                result[@event].Add(assemblyType);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
