using Barber.Domain.Entities;
using Barber.Domain.Repositories;
using Barber.Application.DTOs;

namespace Barber.Application.Services;

public class ClientService
{
    private readonly IUserRepository _userRepo;

    public ClientService(IUserRepository userRepo) => _userRepo = userRepo;

    public async Task<object> RegisterClientAsync(CreateClientRequest request)
    {
        // 1. Validação de Negócio
        if (await _userRepo.EmailExistsAsync(request.Email))
            throw new Exception("Este e-mail já está em uso.");

        // 2. Criar a entidade User (Regra de Autenticação)
        var newUser = new User(request.Name, request.Email, request.Password, "client");
        
        // 3. Persistir
        await _userRepo.AddAsync(newUser);
        await _userRepo.SaveChangesAsync();

        return new { 
            Message = "Cliente cadastrado com sucesso!", 
            UserId = newUser.Id 
        };
    }
}