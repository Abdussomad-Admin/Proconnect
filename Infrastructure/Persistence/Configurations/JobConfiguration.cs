using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Job");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.WorkPlaceType)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.ExperienceLevel)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.Currency)
            .HasMaxLength(10);

        builder.Property(x => x.Location)
            .HasMaxLength(150);

        builder.Property(x => x.MinSalary)
            .HasPrecision(18, 2);

        builder.Property(x => x.MaxSalary)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.DateCreated)
    .IsRequired();

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RecruiterProfile)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.RecruiterProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.JobCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}