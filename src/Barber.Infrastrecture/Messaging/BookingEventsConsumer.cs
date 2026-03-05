using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Barber.Infrastructure.Messaging;

public class BookingEventsConsumer : BackgroundService
{
    private const string DefaultBookingQueue = "booking-events";
    private readonly ILogger<BookingEventsConsumer> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName;

    public BookingEventsConsumer(IConfiguration configuration, ILogger<BookingEventsConsumer> logger)
    {
        _logger = logger;
        _queueName = configuration["RabbitMq:BookingQueue"] ?? DefaultBookingQueue;

        _connectionFactory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (_, eventArgs) =>
                {
                    try
                    {
                        var payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                        using var json = JsonDocument.Parse(payload);

                        var eventType = json.RootElement.TryGetProperty("EventType", out var eventTypeElement)
                            ? eventTypeElement.GetString() ?? "unknown"
                            : "unknown";

                        var orderId = json.RootElement.TryGetProperty("OrderId", out var orderIdElement)
                            ? orderIdElement.GetString()
                            : null;

                        _logger.LogInformation(
                            "Evento processado da fila {Queue}. EventType={EventType} OrderId={OrderId}",
                            _queueName,
                            eventType,
                            orderId ?? "n/a");

                        channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Falha ao processar mensagem da fila {Queue}.", _queueName);
                        channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
                _logger.LogInformation("Consumer RabbitMQ ativo na fila {Queue}.", _queueName);

                while (!stoppingToken.IsCancellationRequested && connection.IsOpen)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no consumer RabbitMQ. Nova tentativa em 5 segundos.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
