using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public interface IStoredEventDispatcher
    {
        EventDispatchResult Dispatch(dynamic evnt);
    }
}
