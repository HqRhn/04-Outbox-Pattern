using Microsoft.EntityFrameworkCore;
using OutboxPublisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region rest endpoint implementations
app.MapGet("/notification", async (OutboxRepository db) =>
    await db.OutboxMessages.ToListAsync()
)
.WithName("GetNotifications");

#endregion
app.Run();
