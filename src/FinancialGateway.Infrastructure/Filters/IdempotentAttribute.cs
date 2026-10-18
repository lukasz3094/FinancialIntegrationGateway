using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FinancialGateway.Core.Entities;
using FinancialGateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialGateway.Infrastructure.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IdempotentAttribute : Attribute, IAsyncActionFilter
{
	private const string IdempotencyHeaderName = "X-Idempotency-Key";

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		// 1. Sprawdzenie obecności nagłówka X-Idempotency-Key
		if (!context.HttpContext.Request.Headers.TryGetValue(IdempotencyHeaderName, out var idempotencyKeyValues) ||
			string.IsNullOrWhiteSpace(idempotencyKeyValues.FirstOrDefault()))
		{
			context.Result = new BadRequestObjectResult(new
			{
				Error = "Missing_Idempotency_Header",
				Message = $"Wymagany jest nagłówek '{IdempotencyHeaderName}' dla zapewnienia idempotencji operacji."
			});
			return;
		}

		var idempotencyKey = idempotencyKeyValues.First()!;
		var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

		// 2. Sprawdzenie, czy klucz istnieje już w bazie PostgreSQL
		var existingRecord = await dbContext.IdempotencyRecords
			.AsNoTracking()
			.FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey);

		if (existingRecord != null)
		{
			// Zwracamy zbuforowaną odpowiedź z bazy
			context.Result = new ContentResult
			{
				Content = existingRecord.ResponseBody,
				ContentType = "application/json",
				StatusCode = existingRecord.StatusCode
			};
			return;
		}

		// 3. Jeśli klucza nie ma, wykonujemy logikę kontrolera
		var executedContext = await next();

		// 4. Po wykonaniu metody zapisujemy wynik do tabeli IdempotencyRecords
		if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode.HasValue)
		{
			var responseJson = JsonSerializer.Serialize(objectResult.Value);

			var record = new IdempotencyRecord
			{
				IdempotencyKey = idempotencyKey,
				RequestHash = ComputeHash(context.HttpContext.Request),
				ResponseBody = responseJson,
				StatusCode = objectResult.StatusCode.Value,
				CreatedAt = DateTime.UtcNow
			};

			dbContext.IdempotencyRecords.Add(record);
			await dbContext.SaveChangesAsync();
		}
	}

	private static string ComputeHash(HttpRequest request)
	{
		using var sha256 = SHA256.Create();
		var rawData = $"{request.Method}:{request.Path}";
		var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
		return Convert.ToBase64String(bytes);
	}
}
