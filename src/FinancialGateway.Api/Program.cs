using FinancialGateway.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Brak connection stringa 'DefaultConnection' w appsettings.json");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(connectionString));

// 1. Dodanie obsługi Kontrolerów oraz Swaggera
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Konfiguracja MassTransit 8.3.6
builder.Services.AddMassTransit(x =>
{
	x.UsingRabbitMq((context, cfg) =>
	{
		var configuration = context.GetRequiredService<IConfiguration>();
		var host = configuration["RabbitMQ:Host"] ?? "localhost";
		var user = configuration["RabbitMQ:Username"] ?? "guest";
		var pass = configuration["RabbitMQ:Password"] ?? "guest";

		cfg.Host(host, "/", h =>
		{
			h.Username(user);
			h.Password(pass);
		});
	});
});

var app = builder.Build();

// 3. Rejestracja Swagger UI
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
