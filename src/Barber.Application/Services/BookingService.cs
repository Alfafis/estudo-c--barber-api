using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Application.DTOs;
using Barber.Application.Messaging;
using Barber.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Barber.Application.Services;

public class BookingService
{
    private const string DefaultBookingQueue = "booking-events";
    private readonly IAppointmentRepository _repo;
    private readonly BarberDbContext _context;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BookingService> _logger;
    private readonly string _queueName;

    public BookingService(
        IAppointmentRepository repo,
        BarberDbContext context,
        IEventPublisher eventPublisher,
        ILogger<BookingService> logger,
        IConfiguration configuration)
    {
        _repo = repo;
        _context = context;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _queueName = configuration["RabbitMq:BookingQueue"] ?? DefaultBookingQueue;
    }

    public async Task<object> BookServiceAsync(CreateAppointmentRequest request)
    {
        var service = await _context.Services.FindAsync(request.ServiceId);
        if (service == null) throw new Exception("Serviço não encontrado.");

        var endTime = request.StartTime.AddMinutes(service.DurationMinutes);

        var hasConflict = await _repo.HasConflictAsync(request.BarberId, request.StartTime, endTime);
        if (hasConflict) throw new Exception("O barbeiro já possui um agendamento neste horário.");

        var order = new Order(
            request.ClientId,
            request.BarberId,
            request.ServiceId,
            service.Price,
            request.StartTime,
            endTime
        );

        await _repo.AddAsync(order);
        await _repo.SaveChangesAsync();

        await TryPublishEventAsync(new
        {
            EventType = "booking.created",
            OrderId = order.Id,
            order.ClientId,
            order.BarberId,
            order.ServiceId,
            order.StartTime,
            order.EndTime,
            order.TotalAmount,
            OccurredAt = DateTimeOffset.UtcNow
        });

        return new
        {
            OrderId = order.Id,
            Resumo = $"{service.Name} agendado com sucesso!",
            Horario = order.StartTime.ToOffset(TimeSpan.FromHours(-3)).ToString("dd/MM/yyyy HH:mm")
        };
    }
    public async Task<object> CancelBookingAsync(Guid orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null) throw new Exception("Agendamento não encontrado.");
        
        if (order.StartTime < DateTimeOffset.UtcNow)
            throw new Exception("Não é possível cancelar um agendamento passado.");

        order.UpdateStatus("cancelled"); 
        await _context.SaveChangesAsync();

        await TryPublishEventAsync(new
        {
            EventType = "booking.cancelled",
            OrderId = order.Id,
            order.ClientId,
            order.BarberId,
            order.ServiceId,
            order.StartTime,
            order.EndTime,
            order.TotalAmount,
            OccurredAt = DateTimeOffset.UtcNow
        });

        return new { Message = "Agendamento cancelado com sucesso." };
    }

    private async Task TryPublishEventAsync(object message)
    {
        try
        {
            await _eventPublisher.PublishAsync(_queueName, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao publicar evento de agendamento no RabbitMQ.");
        }
    }
}