using EventStore.Subscriber.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace EventStore.Subscriber.Serialization
{
    public class StoredEventSerializer : IStoredEventSerializer
    {
        private readonly Assembly _eventTypesAssembly;

        public StoredEventSerializer(Assembly eventTypesAssembly)
        {
            _eventTypesAssembly = eventTypesAssembly;
        }
        public object As(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(object);
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = new EventTypeBinder(_eventTypesAssembly)
            };

            return JsonConvert.DeserializeObject(json, settings);
        }
    }

    public class EventTypeBinder : ISerializationBinder
    {
        private readonly Assembly _eventTypesAssembly;

        public EventTypeBinder(Assembly eventTypeAssembly)
        {
            _eventTypesAssembly = eventTypeAssembly ?? throw new ArgumentNullException(nameof(eventTypeAssembly));
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.DeclaringType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            var assemblyEventType = _eventTypesAssembly
                .GetTypes()
                .FirstOrDefault(x => x.Name == typeName.Split('.').LastOrDefault());

            return assemblyEventType;
        }
    }
}
