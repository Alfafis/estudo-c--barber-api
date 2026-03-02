namespace Barber.Application.DTOs;

public record CreateBarberRequest(
    string Name, 
    string Email, 
    string Password, 
    string Specialty, 
    string DocumentId);