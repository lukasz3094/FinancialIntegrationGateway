using FinancialGateway.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FinancialGateway.Worker.Consumers;

public class SubmitLoanApplicationConsumer : IConsumer<SubmitLoanApplicationCommand>
{
	private readonly ILogger<SubmitLoanApplicationConsumer> _logger;

	public SubmitLoanApplicationConsumer(ILogger<SubmitLoanApplicationConsumer> logger)
	{
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SubmitLoanApplicationCommand> context)
	{
		var message = context.Message;

		_logger.LogInformation(
			"|RECEIVED| Odebrano wniosek: {ApplicationId} | CorrelationID: {CorrelationId} | NIP: {TaxId} | Kwota: {Amount} PLN",
			message.ApplicationId,
			message.CorrelationId,
			message.ApplicantTaxId,
			message.Amount);

		// Symulacja czasochłonnego przetwarzania (np. weryfikacja w systemach bankowych)
		await Task.Delay(2000);

		_logger.LogInformation(
			"|SUCCESS| Wniosek {ApplicationId} został przetworzony pomyślnie.",
			message.ApplicationId);
	}
}
