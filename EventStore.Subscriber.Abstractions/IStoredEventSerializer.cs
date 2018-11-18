using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public interface IStoredEventSerializer
    {
        object As(string json);
    }
}
