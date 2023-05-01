using Basket.API.Repositories;
using Microsoft.OpenApi.Models;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddScoped<IBasketRepository, BasketRepository>();
		builder.Services.AddStackExchangeRedisCache(o =>
		{
			o.Configuration = builder.Configuration["CacheSettings:ConnectionString"];
		});

		builder.Services.AddControllers();
		builder.Services.AddSwaggerGen(o =>
			o.SwaggerDoc("Basket.API", new OpenApiInfo() { Title = "Basket.API", Version = "1.0" }));

		var app = builder.Build();

		// Configure the HTTP request pipeline.

		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/Basket.API/swagger.json", "Basket.API"));
		}

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}