using FinancialGateway.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialGateway.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
	public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Konfiguracja encji LoanApplication
		modelBuilder.Entity<LoanApplication>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.CorrelationId).IsUnique();
			entity.Property(e => e.Amount).HasPrecision(18, 2);
			entity.Property(e => e.ApplicantTaxId).HasMaxLength(20).IsRequired();
			entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
		});

		// Konfiguracja encji IdempotencyRecord
		modelBuilder.Entity<IdempotencyRecord>(entity =>
		{
			entity.HasKey(e => e.IdempotencyKey);
			entity.Property(e => e.IdempotencyKey).HasMaxLength(128);
			entity.Property(e => e.RequestHash).HasMaxLength(256).IsRequired();
		});
	}
}
