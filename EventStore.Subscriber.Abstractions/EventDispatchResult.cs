using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public class EventDispatchResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
