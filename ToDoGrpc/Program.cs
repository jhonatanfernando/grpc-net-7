using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Data;
using ToDoGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenLocalhost(5000, p => p.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=ToDoDatabase.db"));

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ToDoService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
