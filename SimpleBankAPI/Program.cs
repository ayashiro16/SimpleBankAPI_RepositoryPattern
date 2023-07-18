using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Repositories;
using AccountServices = SimpleBankAPI.Services.AccountServices;
using AccountContext = SimpleBankAPI.Data.AccountContext;
using Account = SimpleBankAPI.Models.Entities.Account;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddDbContext<AccountContext>(opt =>
    opt.UseInMemoryDatabase("Accounts"));
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
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