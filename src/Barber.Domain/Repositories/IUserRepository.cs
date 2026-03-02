using Barber.Domain.Entities;

namespace Barber.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task SaveChangesAsync();
}