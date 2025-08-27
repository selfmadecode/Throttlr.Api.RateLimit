using RateLimit.Throttlr.Core;
using RateLimit.Throttlr.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//5 requests per 10 seconds
builder.Services.AddRateLimiting(
    new RateLimitPolicy(
        "global",
        limit: 5,
        window: TimeSpan.FromSeconds(10)
    ),
    limiterType: RateLimiterType.SlidingWindow
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiting();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
