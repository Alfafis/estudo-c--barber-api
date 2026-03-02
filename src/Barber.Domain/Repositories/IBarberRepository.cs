using Barber.Domain.Entities;

namespace Barber.Domain.Repositories;

public interface IBarberRepository
{
    Task AddAsync(global::Barber.Domain.Entities.Barber barber);
    Task SaveChangesAsync();
}