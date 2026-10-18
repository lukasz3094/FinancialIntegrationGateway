$bash
# 1. Tworzenie solucji
dotnet new sln -n FinancialIntegrationGateway

# 2. Tworzenie projektów w katalogu src/
dotnet new webapi -n FinancialGateway.Api -o src/FinancialGateway.Api --framework net8.0
dotnet new worker -n FinancialGateway.Worker -o src/FinancialGateway.Worker --framework net8.0
dotnet new classlib -n FinancialGateway.Core -o src/FinancialGateway.Core --framework net8.0
dotnet new classlib -n FinancialGateway.Infrastructure -o src/FinancialGateway.Infrastructure --framework net8.0

# 3. Dodanie projektów do solucji
dotnet sln add src/FinancialGateway.Api/FinancialGateway.Api.csproj
dotnet sln add src/FinancialGateway.Worker/FinancialGateway.Worker.csproj
dotnet sln add src/FinancialGateway.Core/FinancialGateway.Core.csproj
dotnet sln add src/FinancialGateway.Infrastructure/FinancialGateway.Infrastructure.csproj

# 4. Ustawienie referencji między projektami (Przeznaczenie warstw)
# Api -> Infrastructure & Core
dotnet add src/FinancialGateway.Api/FinancialGateway.Api.csproj reference src/FinancialGateway.Infrastructure/FinancialGateway.Infrastructure.csproj
dotnet add src/FinancialGateway.Api/FinancialGateway.Api.csproj reference src/FinancialGateway.Core/FinancialGateway.Core.csproj

# Worker -> Infrastructure & Core
dotnet add src/FinancialGateway.Worker/FinancialGateway.Worker.csproj reference src/FinancialGateway.Infrastructure/FinancialGateway.Infrastructure.csproj
dotnet add src/FinancialGateway.Worker/FinancialGateway.Worker.csproj reference src/FinancialGateway.Core/FinancialGateway.Core.csproj

# Infrastructure -> Core
dotnet add src/FinancialGateway.Infrastructure/FinancialGateway.Infrastructure.csproj reference src/FinancialGateway.Core/FinancialGateway.Core.csproj

# 5. Instalacja kluczowych paczek NuGet dla MassTransit, RabbitMQ i EF Core
dotnet add src/FinancialGateway.Api/FinancialGateway.Api.csproj package MassTransit.RabbitMQ
dotnet add src/FinancialGateway.Worker/FinancialGateway.Worker.csproj package MassTransit.RabbitMQ

dotnet add src/FinancialGateway.Infrastructure/FinancialGateway.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/FinancialGateway.Infrastructure/FinancialGateway.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL