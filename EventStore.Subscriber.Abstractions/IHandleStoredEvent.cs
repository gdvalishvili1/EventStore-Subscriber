using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public interface IHandleStoredEvent<TEvent>
    {
        void Handle(TEvent @event, StoredEventDispatchContext storedEventDispatchContext);
    }
}
