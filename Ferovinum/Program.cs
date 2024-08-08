using Ferovinum.Services;
using Ferovinum.Services.Contracts;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ferovinum.Services.Mappings;
using Ferovinum.DatabaseInitializer;
using Ferovinum.MiddlewareHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddDbContext<TransactionsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TransactionsContext")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();

builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

DatabaseInitializer.Seed(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalErrorHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
