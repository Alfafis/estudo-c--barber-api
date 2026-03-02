using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Application.DTOs;
using Barber.Infrastructure.Context;

namespace Barber.Application.Services;

public class BookingService
{
    private readonly IAppointmentRepository _repo;
    private readonly BarberDbContext _context;

    public BookingService(IAppointmentRepository repo, BarberDbContext context)
    {
        _repo = repo;
        _context = context;
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
        return new { Message = "Agendamento cancelado com sucesso." };
    }
}