using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Application.DTOs;
using Barber.Application.Messaging;
using Barber.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
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
        var clientExists = await _context.Clients.AnyAsync(x => x.Id == request.ClientId);
        if (!clientExists)
            throw new Exception("Cliente não encontrado. Use o ID da tabela clients, não o user_id.");

        var barberExists = await _context.Barbers.AnyAsync(x => x.Id == request.BarberId);
        if (!barberExists)
            throw new Exception("Barbeiro não encontrado. Use o ID da tabela barbers, não o user_id.");

        var service = await _context.Services.FindAsync(request.ServiceId);
        if (service == null) throw new Exception("Serviço não encontrado.");

        // PostgreSQL timestamptz expects UTC values with Npgsql.
        var startTimeUtc = request.StartTime.ToUniversalTime();
        var endTimeUtc = startTimeUtc.AddMinutes(service.DurationMinutes);

        var hasConflict = await _repo.HasConflictAsync(request.BarberId, startTimeUtc, endTimeUtc);
        if (hasConflict) throw new Exception("O barbeiro já possui um agendamento neste horário.");

        var order = new Order(
            request.ClientId,
            request.BarberId,
            request.ServiceId,
            service.Price,
            startTimeUtc,
            endTimeUtc
        );

        await _repo.AddAsync(order);
        try
        {
            await _repo.SaveChangesAsync();
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Falha ao salvar agendamento.");
            throw new Exception(dbEx.InnerException?.Message ?? "Falha ao salvar agendamento no banco de dados.");
        }

        var published = await TryPublishEventAsync(new
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
            Horario = order.StartTime.ToOffset(TimeSpan.FromHours(-3)).ToString("dd/MM/yyyy HH:mm"),
            Published = published
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

        var published = await TryPublishEventAsync(new
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

        return new
        {
            Message = "Agendamento cancelado com sucesso.",
            Published = published
        };
    }

    private async Task<bool> TryPublishEventAsync(object message)
    {
        try
        {
            await _eventPublisher.PublishAsync(_queueName, message);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao publicar evento de agendamento no RabbitMQ.");
            return false;
        }
    }
}