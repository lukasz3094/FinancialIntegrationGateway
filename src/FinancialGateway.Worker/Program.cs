using FinancialGateway.Worker.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

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
