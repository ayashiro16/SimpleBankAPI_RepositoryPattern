using AccountContext = SimpleBankAPI.Data.AccountContext;
using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddDbContext<AccountContext>(opt =>
    opt.UseInMemoryDatabase("Accounts"));
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddTransient<ISavableCollection, AccountContext>();
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