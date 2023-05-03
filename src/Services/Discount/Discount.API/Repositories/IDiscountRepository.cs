using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories;

public interface IDiscountRepository
{
	Task<Coupon?> GetDiscount(string productName);
	Task<bool> CreateDiscount(Coupon coupon);
	Task<bool> UpdateDiscount(Coupon coupon);
	Task<bool> DeleteDiscount(string productName);
}

public class DiscountRepository : IDiscountRepository
{
	private readonly IConfiguration _configuration;

	public DiscountRepository(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	public async Task<Coupon?> GetDiscount(string productName)
	{
		// init connection
		await using var connection = new NpgsqlConnection(_configuration["DatabaseSettings:ConnectionString"]);
		var coupon = await
			connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName",
				new { ProductName = productName });
		return coupon;
	}

	public async Task<bool> CreateDiscount(Coupon coupon)
	{
		await using var connection = new NpgsqlConnection(_configuration["DatabaseSettings:ConnectionString"]);
		int affected = await connection.ExecuteAsync(
			"INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", 
			new {ProductName=coupon.ProductName, Description=coupon.Description, Amount = coupon.Amount});
		if (affected == 0)
		{
			return false;
		}
		return true;
	}

	public async Task<bool> UpdateDiscount(Coupon coupon)
	{
		await using var connection = new NpgsqlConnection(_configuration["DatabaseSettings:ConnectionString"]);
		int affected = await connection.ExecuteAsync(
			"UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
			new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id=coupon.Id });
		if (affected == 0)
		{
			return false;
		}
		return true;
	}

	public async Task<bool> DeleteDiscount(string productName)
	{
		await using var connection = new NpgsqlConnection(_configuration["DatabaseSettings:ConnectionString"]);
		var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName",
			new { ProductName = productName });
		if (affected == 0)
		{
			return false;
		}

		return true;
	}
}