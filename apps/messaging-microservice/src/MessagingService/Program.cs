using MessagingService.Messaging;
using MessagingService.Repositories;
using MessagingService.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Register services for dependency injection ---
// When a class asks for IMessageRepository, give it the in-memory version
builder.Services.AddSingleton<IMessageRepository, InMemoryMessageRepository>();

// When a class asks for IMessageService, give it our MessageService
builder.Services.AddScoped<IMessageService, MessageService>();

// Register our RabbitMQ message client (from the previous task)
builder.Services.AddMessageClient("host=localhost");

// Register controllers (tells .NET to look for classes with [ApiController])
builder.Services.AddControllers();

// Register Swagger for API documentation and testing UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// In development, enable Swagger UI at /swagger so we can test our endpoints visually
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controller routes (connects the [Route] attributes to actual URLs)) 
app.MapControllers();

app.Run();