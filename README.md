# Financial Integration Gateway (.NET 8)

Asynchroniczny system integracyjny pośredniczący w procesie weryfikacji wniosków kredytowych.

## Architecture & Tech Stack

- **Framework:** .NET 8 (Web API + Background Worker)
- **Messaging:** RabbitMQ + MassTransit (Request-Callback, Correlation ID, Retry, DLQ)
- **Data & Persistence:** PostgreSQL, Entity Framework Core (Idempotency Pattern)
- **Integrations:** REST / OpenAI API, SOAP / XML Mock Service
- **Containerization:** Docker & Docker Compose

## Quick Start

```bash
# 1. Start Infrastructure (RabbitMQ & PostgreSQL)
docker compose -f docker/docker-compose.yml up -d

# 2. Build Solution
dotnet build
