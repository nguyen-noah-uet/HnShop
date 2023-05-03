using Discount.API.Repositories;
using Npgsql;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		
		// Add services to the container.
		builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		
		var app = builder.Build();
		await MigrateDatabase(app);
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseAuthorization();

		app.MapControllers();

		await app.RunAsync();
	}

	private static async Task MigrateDatabase(WebApplication app, int retry=0)
	{
		using var scope = app.Services.CreateScope();
		var configuration = scope.ServiceProvider.GetService<IConfiguration>();
		var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
		try
		{
			logger!.LogInformation("Migrating Postgres SQL database.");
			var connection = new NpgsqlConnection(configuration!["DatabaseSettings:ConnectionString"]);
			connection.Open();
			var command = new NpgsqlCommand()
			{
				Connection = connection,
			};
			if (app.Environment.IsDevelopment())
			{
				
				command.CommandText = "DROP TABLE IF EXISTS Coupon";
				await command.ExecuteNonQueryAsync();
				command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
				await command.ExecuteNonQueryAsync();

				command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150)";
				await command.ExecuteNonQueryAsync();

				command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung S23', 'Samsung Discount', 99);";
				await command.ExecuteNonQueryAsync();
				logger!.LogInformation("Migrate Postgres SQL database successfully.");
			}
			else
			{
				command.CommandText = @"CREATE TABLE IF NOT EXISTS Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
				await command.ExecuteNonQueryAsync();
			}
		}
		catch (Exception e)
		{
			logger!.LogError("An error occurred while trying to migrate database.");
			logger!.LogError(e.Message);
			if (retry < 10)
			{
				Thread.Sleep(1000);
				await MigrateDatabase(app, ++retry);
			}
			else
			{
				logger!.LogInformation("Program exiting.");
				Environment.Exit(1);
			}
		}
	}
}