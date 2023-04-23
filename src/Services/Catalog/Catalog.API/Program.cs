using Catalog.API.Data;
using Catalog.API.Repositories;
using Microsoft.OpenApi.Models;

namespace Catalog.API;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		builder.Services.AddSwaggerGen(o =>
			o.SwaggerDoc("Catalog.API", new OpenApiInfo() { Title = "Catalog.API", Version = "1.0" }));
		builder.Services.AddScoped<ICatalogContext, CatalogContext>();
		builder.Services.AddScoped<IProductRepository, ProductRepository>();


		var app = builder.Build();

		// Configure the HTTP request pipeline.

		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/Catalog.API/swagger.json", "Catalog.API"));
		}

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}