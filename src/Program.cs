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

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapAppointmentEndpoints();
app.MapClientEndpoints();
app.MapBarberEndpoints();
app.MapBookingEndpoints();
app.MapDashboardEndpoints();
app.MapControllers();


app.Run();
