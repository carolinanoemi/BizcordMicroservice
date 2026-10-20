# Messaging Service

Microservice for the Bizcord platform.

Manages sending, storing, editing, and deleting messages in real time. Part of a larger microservices system where each team owns one service.

## Project Structure

```
apps/messaging-microservice/
  src/MessagingService/
    Controllers/          - REST API endpoints
    Domain/
      Entities/           - Message entity with factory method
      Events/             - Internal domain events
      Exceptions/         - DomainException for business rules
    DTOs/                 - Request validation (CreateMessageRequest, UpdateMessageRequest)
    Messaging/            - RabbitMQ message client (IMessageClient, RabbitMqMessageClient)
    Repositories/         - In-memory data storage
    Services/             - Business logic layer
    Shared/               - Public-facing models (MessageDto)
  tests/MessagingService.Tests/
    Helpers/              - FakeMessageClient, TestWebApplicationFactory
    MessageTests.cs       - Unit tests for Message.Create()
    ContractTests.cs      - Provider contract test for MessagePostedEvent
    IntegrationTests.cs   - Full round-trip API test (POST + GET)
packages/Shared.Contracts/
    Events/               - MessagePostedEvent (shared contract used by all teams)
```

## Architecture

- **Layered architecture**: Controller -> Service -> Repository
- **Domain model**: Message entity with Message.Create() factory method and validation
- **Messaging**: RabbitMQ via EasyNetQ v8 for publishing events to other services
- **Shared contract**: MessagePostedEvent is the event all other services depend on

## API Endpoints

| Method | Endpoint                          | Description                        |
|--------|-----------------------------------|------------------------------------|
| GET    | /api/messages/{id}                | Get a single message by ID         |
| GET    | /api/messages/channel/{channelId} | Get all messages in a channel      |
| POST   | /api/messages                     | Send a new message                 |
| PUT    | /api/messages/{id}                | Edit a message                     |
| DELETE | /api/messages/{id}                | Soft-delete a message              |

## How to Run

### With dotnet CLI

```bash
dotnet run --project apps/messaging-microservice/src/MessagingService
```

### With Docker Compose

```bash
cd apps/messaging-microservice
docker compose up --build
```

This starts both RabbitMQ and the Messaging Service. Swagger UI is available at http://localhost:5280/swagger when running in Docker.

## How to Test

```bash
dotnet test apps/messaging-microservice/tests/MessagingService.Tests
```

Runs all 5 tests:
- 3 unit tests (Message.Create validation)
- 1 provider contract test (MessagePostedEvent fields)
- 1 integration test (POST + GET round trip)

## Tech Stack

- .NET 9 / ASP.NET Core Web API
- RabbitMQ with EasyNetQ v8
- Swashbuckle (Swagger) for API documentation
- xUnit + FluentAssertions for testing
- Docker + Docker Compose
