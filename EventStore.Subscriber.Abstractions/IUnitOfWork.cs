using System;
using System.Collections.Generic;
using System.Text;

namespace EventStore.Subscriber.Abstractions
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }
}
