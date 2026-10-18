using FinancialGateway.Core.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FinancialGateway.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
	private readonly IPublishEndpoint _publishEndpoint;

	public LoansController(IPublishEndpoint publishEndpoint)
	{
		_publishEndpoint = publishEndpoint;
	}

	[HttpPost]
	public async Task<IActionResult> SubmitLoan(
		[FromBody] SubmitLoanRequest request,
		[FromHeader(Name = "X-Correlation-ID")] string? correlationId)
	{
		var activeCorrelationId = string.IsNullOrWhiteSpace(correlationId)
			? Guid.NewGuid().ToString()
			: correlationId;

		var command = new SubmitLoanApplicationCommand
		{
			ApplicationId = Guid.NewGuid(),
			CorrelationId = activeCorrelationId,
			ApplicantTaxId = request.ApplicantTaxId,
			Amount = request.Amount,
			TermInMonths = request.TermInMonths,
			PurposeDescription = request.PurposeDescription,
			SubmittedAt = DateTime.UtcNow
		};

		await _publishEndpoint.Publish(command);

		return Accepted($"/api/loans/{command.ApplicationId}", new
		{
			command.ApplicationId,
			command.CorrelationId,
			Status = "Processing",
			Message = "Wniosek został przyjęty do asynchronicznego przetwarzania."
		});
	}
}

public record SubmitLoanRequest(
	string ApplicantTaxId,
	decimal Amount,
	int TermInMonths,
	string PurposeDescription);
