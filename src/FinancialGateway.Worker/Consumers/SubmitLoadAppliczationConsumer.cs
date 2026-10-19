using FinancialGateway.Core.Contracts;
using FinancialGateway.Core.Entities;
using FinancialGateway.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinancialGateway.Worker.Consumers;

public class SubmitLoanApplicationConsumer : IConsumer<SubmitLoanApplicationCommand>
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ILogger<SubmitLoanApplicationConsumer> _logger;

	public SubmitLoanApplicationConsumer(
		ApplicationDbContext dbContext,
		ILogger<SubmitLoanApplicationConsumer> logger)
	{
		_dbContext = dbContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<SubmitLoanApplicationCommand> context)
	{
		var msg = context.Message;

		_logger.LogInformation(
			"|PROCESSING| Rozpoczynam przetwarzanie wniosku {ApplicationId} | CorrelationID: {CorrelationId}",
			msg.ApplicationId,
			msg.CorrelationId);

		// 1. Utworzenie lub pobranie wniosku w bazie danych PostgreSQL
		var loanApplication = await _dbContext.LoanApplications
			.FirstOrDefaultAsync(l => l.Id == msg.ApplicationId);

		if (loanApplication == null)
		{
			loanApplication = new LoanApplication
			{
				Id = msg.ApplicationId,
				CorrelationId = msg.CorrelationId,
				ApplicantTaxId = msg.ApplicantTaxId,
				Amount = msg.Amount,
				TermInMonths = msg.TermInMonths,
				PurposeDescription = msg.PurposeDescription,
				Status = "Processing",
				CreatedAt = DateTime.UtcNow
			};

			_dbContext.LoanApplications.Add(loanApplication);
			await _dbContext.SaveChangesAsync();
		}

		// 2. Symulacja decyzji biznesowej (np. reguły biznesowe / ocena ryzyka)
		await Task.Delay(2000); // Symulacja opóźnienia sieciowego/obliczeniowego

		bool isApproved = msg.Amount <= 100000; // Akceptujemy wnioski do 100 tys. PLN
		loanApplication.Status = isApproved ? "Approved" : "Rejected";
		loanApplication.DecisionReason = isApproved
			? "Wniosek spełnia automatyczne kryteria kwotowe."
			: "Kwota wniosku przekracza automatyczny limit 100 000 PLN.";
		loanApplication.ProcessedAt = DateTime.UtcNow;

		await _dbContext.SaveChangesAsync();

		_logger.LogInformation(
			"|COMPLETED| Wniosek {ApplicationId} zaktualizowany w PostgreSQL ze stanem: {Status}",
			msg.ApplicationId,
			loanApplication.Status);

		// 3. WZORZEC REQUEST-CALLBACK: Wysyłka powiadomienia zwrotnego (Callback/Webhook)
		await SendCallbackNotificationAsync(loanApplication);
	}

	private async Task SendCallbackNotificationAsync(LoanApplication loan)
	{
		// W pełnym wdrożeniu wykonuje się tutaj HTTP POST na adres Webhooka klienta
		_logger.LogInformation(
			"|CALLBACK SENT| Wysyłanie powiadomienia Callback dla CorrelationID: {CorrelationId} -> Status: {Status}, Powód: {Reason}",
			loan.CorrelationId,
			loan.Status,
			loan.DecisionReason);

		await Task.CompletedTask;
	}
}
