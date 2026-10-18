namespace FinancialGateway.Core.Contracts;

public record class SubmitLoanApplicationCommand
{
	public required Guid ApplicationId { get; init; }
	public required string CorrelationId { get; init; }
	public required string ApplicantTaxId { get; init; } // NIP / PESEL
	public required decimal Amount { get; init; }
	public required int TermInMonths { get; init; }
	public required string PurposeDescription { get; init; }
	public required DateTime SubmittedAt { get; init; }
}
