namespace Barber.Application.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(string queueName, object message, CancellationToken cancellationToken = default);
}
