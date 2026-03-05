using Barber.Infrastructure.Context;
using Barber.Application.Messaging;
using Barber.Domain.Repositories;
using Barber.Infrastructure.Repositories;
using Barber.Infrastructure.Messaging;
using Barber.Application.Services;
using Microsoft.EntityFrameworkCore;
using Barber.Api.Endpoints;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BarberDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddControllers();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IBarberRepository, BarberRepository>();
builder.Services.AddScoped<BarberService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddHostedService<BookingEventsConsumer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddSingleton(new NpgsqlDataSourceBuilder(connectionString).Build());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Acesse em http://localhost:5000/swagger
}

app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    var request = statusCodeContext.HttpContext.Request;

    if (response.StatusCode is StatusCodes.Status404NotFound or StatusCodes.Status405MethodNotAllowed)
    {
        response.ContentType = "application/json";

        await response.WriteAsJsonAsync(new
        {
            error = response.StatusCode == StatusCodes.Status404NotFound
                ? "Rota nao encontrada."
                : "Metodo nao permitido para esta rota.",
            method = request.Method,
            path = request.Path.Value,
            statusCode = response.StatusCode,
            timestamp = DateTimeOffset.UtcNow
        });
    }
});

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapAppointmentEndpoints();
app.MapClientEndpoints();
app.MapBarberEndpoints();
app.MapBookingEndpoints();
app.MapDashboardEndpoints();
app.MapShopEndpoints();
app.MapUserEndpoints();
app.MapServiceEndpoints();
app.MapOrderEndpoints();
app.MapTransactionEndpoints();
app.MapControllers();

app.MapFallback((HttpContext context) =>
{
    return Results.NotFound(new
    {
        error = "Rota nao encontrada.",
        method = context.Request.Method,
        path = context.Request.Path.Value,
        statusCode = StatusCodes.Status404NotFound,
        timestamp = DateTimeOffset.UtcNow
    });
});


app.Run();
