namespace Barber.Application.DTOs;

public record CreateClientRequest(
    string Name, 
    string Email, 
    string Password, 
    string Phone, 
    DateTime? BirthDate);