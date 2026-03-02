using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Application.DTOs;
using Barber.Infrastructure.Context;

namespace Barber.Application.Services;

public class BarberService
{
    private readonly IUserRepository _userRepo;
    private readonly IBarberRepository _barberRepo;
    private readonly BarberDbContext _context;

    public BarberService(IUserRepository userRepo, IBarberRepository barberRepo, BarberDbContext context)
    {
        _userRepo = userRepo;
        _barberRepo = barberRepo;
        _context = context;
    }

    public async Task<object> RegisterBarberAsync(CreateBarberRequest request)
    {
        if (await _userRepo.EmailExistsAsync(request.Email))
            throw new Exception("E-mail já cadastrado.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = new User(request.Name, request.Email, request.Password, "barber");
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var barber = new global::Barber.Domain.Entities.Barber(user.Id, request.DocumentId, request.Specialty);
            await _barberRepo.AddAsync(barber);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new { BarberId = barber.Id, Name = user.Name };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}