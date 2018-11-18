using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public class StoredEventDispatchContext
    {
        public StoredEventDispatchContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public IServiceProvider ServiceProvider { get; }
    }
}
