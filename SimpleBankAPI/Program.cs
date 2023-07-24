using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Clients;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Repositories;
using SimpleBankAPI.Services;
using SimpleBankAPI.Data;
using SimpleBankAPI.Models.Entities;
using SimpleBankAPI.Factories;

var builder = WebApplication.CreateBuilder(args);
var currencyKey = builder.Configuration.GetValue<string>("CURRENCY_API_KEY");

var services = builder.Services;

services.AddControllers();
services.AddDbContext<AccountContext>(opt =>
    opt.UseInMemoryDatabase("Accounts"));
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddSingleton<ICurrencyRate, CurrencyClient>();
services.AddHttpClient<ICurrencyRate, CurrencyClient>(client => 
    client.BaseAddress = new Uri($"https://api.freecurrencyapi.com/v1/latest?apikey={currencyKey}"));
services.AddSingleton<IFactory<IValidator?>, ValidatorFactory>();
services.AddTransient<IAccountsRepository, AccountsRepository>();
services.AddTransient<IAccountsService, AccountsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();