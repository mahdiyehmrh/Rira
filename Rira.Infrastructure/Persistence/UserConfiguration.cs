using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rira.Domain.Users;
using Rira.Domain.Users.ValueObjects;

namespace Rira.Infrastructure.Persistence;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.FirstName)
            .HasConversion(value => value.Value, value => PersonName.FromDatabase(value))
            .HasMaxLength(PersonName.MaxLength)
            .IsRequired();
        
        builder.Property(x => x.LastName)
            .HasConversion(value => value.Value, value => PersonName.FromDatabase(value))
            .HasMaxLength(PersonName.MaxLength)
            .IsRequired();
        
        builder.Property(x => x.NationalCode)
            .HasConversion(value => value.Value, value => NationalCode.FromDatabase(value))
            .HasMaxLength(10)
            .IsRequired();
        
        builder.HasIndex(x => x.NationalCode).IsUnique();
        
        builder.Property(x => x.BirthDate).HasColumnType("date").IsRequired();
    }
}
