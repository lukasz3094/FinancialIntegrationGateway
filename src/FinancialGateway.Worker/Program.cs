using FinancialGateway.Infrastructure.Persistence;
using FinancialGateway.Worker.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// 1. Rejestracja EF Core i DbContext w Workerze
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Brak connection stringa w Workerze");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Rejestracja MassTransit 8.3.6
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitLoanApplicationConsumer>();

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

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
await host.RunAsync();
