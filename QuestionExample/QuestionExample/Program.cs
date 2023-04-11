using Microsoft.EntityFrameworkCore;
using QuestionExample.Data;
using QuestionExample.Function.IFunctions;
using QuestionExample.Function;

var builder = WebApplication.CreateBuilder(args);

//Scaffold-DbContext "Server=10.10.0.15\SQLEXPRESS;Database=SalesRep;TrustServerCertificate=True;User ID=newtype;Password=newtype;MultipleActiveResultSets=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data -Force -Tables FORECAST_ORDER,BRAND,FACTORY

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<SalesRepContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection")));
builder.Services.AddScoped<ISqlExcute, SqlExcute>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
