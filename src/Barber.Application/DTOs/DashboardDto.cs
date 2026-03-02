namespace Barber.Application.DTOs;

public record DashboardDto(
    int TotalAppointments,
    decimal TotalRevenue,
    decimal AverageTicket,
    string Date);