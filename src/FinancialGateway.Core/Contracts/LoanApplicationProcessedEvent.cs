namespace FinancialGateway.Core.Contracts;

public record class LoanApplicationProcessedEvent
{
	public required Guid ApplicationId { get; init; }
	public required string CorrelationId { get; init; }
	public required bool IsApproved { get; init; }
	public required string StatusReason { get; init; }
	public required DateTime ProcessedAt { get; init; }
}
