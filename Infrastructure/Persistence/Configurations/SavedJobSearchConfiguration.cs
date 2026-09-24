using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SavedJobSearchConfiguration : IEntityTypeConfiguration<SavedJobSearch>
{
    public void Configure(EntityTypeBuilder<SavedJobSearch> builder)
    {
        builder.ToTable("SavedJobSearch");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmploymentType).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.WorkPlaceType).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.ExperienceLevel).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.DateCreated).IsRequired();

        builder.HasOne(x => x.ProfessionalProfile)
            .WithMany(p => p.SavedJobSearches)
            .HasForeignKey(x => x.ProfessionalProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}