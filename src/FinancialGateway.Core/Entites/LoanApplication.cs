namespace FinancialGateway.Core.Entities;

public class LoanApplication
{
	public Guid Id { get; set; }
	public required string CorrelationId { get; set; }
	public required string ApplicantTaxId { get; set; }
	public decimal Amount { get; set; }
	public int TermInMonths { get; set; }
	public required string PurposeDescription { get; set; }
	public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
	public string? DecisionReason { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? ProcessedAt { get; set; }
}
