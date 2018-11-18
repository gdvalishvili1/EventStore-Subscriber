using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Subscriber.Abstractions;
using System;
using System.Text;
using System.Threading;

namespace EventStore.Subscriber
{
    public class CatchUpSubscriptionClient
    {
        private IEventStoreConnection _conn;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventPositionRepository _eventPositionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoredEventSerializer _storedEventSerializer;
        private readonly IStoredEventDispatcher _eventDispatcher;

        public CatchUpSubscriptionClient(
            IServiceProvider serviceProvider,
            IEventPositionRepository eventPositionRepository,
            IUnitOfWork unitOfWork,
            IStoredEventSerializer storedEventSerializer,
            IStoredEventDispatcher eventDispatcher
            )
        {
            _serviceProvider = serviceProvider;
            _eventPositionRepository = eventPositionRepository;
            _unitOfWork = unitOfWork;
            _storedEventSerializer = storedEventSerializer;
            _eventDispatcher = eventDispatcher;
        }

        public void Start(string connectionString,
            string username,
            string password,
            int? waitTimeoutMs = null)
        {
            try
            {
                using (_conn = EventStoreConnection.Create(connectionString))
                {
                    _conn.ConnectAsync().Wait();

                    ConnectToSubscription(connectionString, username, password);

                    Console.WriteLine("waiting for events. press enter to exit");

                    if (waitTimeoutMs.HasValue)
                    {
                        Thread.Sleep(waitTimeoutMs.Value);
                    }
                    else
                    {
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: retry or logging
            }
        }

        private void ConnectToSubscription(string connectionString, string username, string password)
        {
            var lastCheckpoint = _eventPositionRepository.LastPosition();

            Position lastPosition = lastCheckpoint == null ? default(Position) : new Position(lastCheckpoint.CommitPosition, lastCheckpoint.PreparePosition);

            _conn.SubscribeToAllFrom(lastPosition, new CatchUpSubscriptionSettings(10, 1000, true, true), (x, resolvedEvent) =>
            {
                if (resolvedEvent.OriginalStreamId.StartsWith("$"))
                    return;

                if (resolvedEvent.Event.Data.Length <= 0)
                    return;

                try
                {
                    var evnt = _storedEventSerializer.As(Encoding.ASCII.GetString(resolvedEvent.Event.Data));

                    if (evnt == null)
                        return;

                    _eventDispatcher.Dispatch(evnt);

                    _eventPositionRepository.StoreLastPosition(
                        resolvedEvent.OriginalPosition.Value.CommitPosition,
                        resolvedEvent.OriginalPosition.Value.PreparePosition
                        );

                    _unitOfWork.SaveChanges();

                    //TODO:logging
                    Console.WriteLine("Received: " + resolvedEvent.Event.EventStreamId + ":" + resolvedEvent.Event.EventNumber);
                }
                catch (Exception ex)
                {
                    //log
                }
            }, userCredentials: new UserCredentials(username, password));
        }
    }
}
