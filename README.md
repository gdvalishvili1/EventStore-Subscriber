# EventStore-Subscriber
EventStore (GES) Subscriber With EventDispatcher and Handlers

# Example

```csharp
public class ExampleAggregateCreatedEvent : IStoredEvent
    {
        public ExampleAggregateCreatedEvent(string state)
        {
            State = state;
        }

        public string State { get; }
    }

 //start listening on every stream change, and dispatch deserialized event to every registered eventHandlers like below 
 new CatchUpSubscriptionClient(
                    serviceProvider,
                    eventPositionRepository,
                    unitOfWork,
                    storedEventSerializer,
                    eventDispatcher
                    ).Start(eventStoreConnString,
                    config.GetValue<string>("EventStore:UserName"),
                    config.GetValue<string>("EventStore:Password"));

 //subscribed event stream changes handled here
 public class ExampleEventStoreEventHandler : IHandleStoredEvent<ExampleAggregateCreatedEvent>
    {
        public void Handle(FakeAggregateCreatedEvent @event, StoredEventDispatchContext storedEventDispatchContext)
        {
            //do work like storedEventDispatchContext.ServiceProvider.GetService<SomeRepository>().Save();
        }
    } 
 ```
