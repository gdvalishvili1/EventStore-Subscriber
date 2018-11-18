using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public interface IEventPositionRepository
    {
        void StoreLastPosition(long commit, long prepare);

        EventPosition LastPosition();
    }
}
