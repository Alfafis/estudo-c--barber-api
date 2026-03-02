using Microsoft.EntityFrameworkCore;
using Barber.Domain.Entities;
using BarberEntity = Barber.Domain.Entities.Barber;

namespace Barber.Infrastructure.Context;

public class BarberDbContext : DbContext
{
    public BarberDbContext(DbContextOptions<BarberDbContext> options) : base(options) { }

    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<User> Users => Set<User>();
    public DbSet<BarberEntity> Barbers => Set<BarberEntity>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}