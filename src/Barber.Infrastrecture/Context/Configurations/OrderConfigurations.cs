using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Barber.Domain.Entities;

namespace Barber.Infrastructure.Context.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.TotalAmount).HasPrecision(10, 2);

        builder.HasIndex(o => o.StartTime);
        
        builder.HasOne<Client>().WithMany().HasForeignKey(o => o.ClientId);
    }
}