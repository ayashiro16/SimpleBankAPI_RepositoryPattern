using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Clients;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Repositories;
using SimpleBankAPI.Services;
using SimpleBankAPI.Data;
using SimpleBankAPI.Models.Entities;
using SimpleBankAPI.Factories;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddDbContext<AccountContext>(opt =>
    opt.UseInMemoryDatabase("Accounts"));
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddSingleton<ICurrencyRate, CurrencyClient>();
services.AddSingleton<IFactory<IValidator?>, ValidatorFactory>();
services.AddTransient<ISavableCollection<Account>, AccountContext>();
services.AddTransient<IAccountRepository, AccountRepository>();
services.AddTransient<IAccountServices, AccountServices>();

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