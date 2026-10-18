namespace FinancialGateway.Core.Entities;

public class IdempotencyRecord
{
	public required string IdempotencyKey { get; set; }
	public required string RequestHash { get; set; }
	public required string ResponseBody { get; set; }
	public int StatusCode { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
