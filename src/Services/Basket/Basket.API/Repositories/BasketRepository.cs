using System.Text.Json;
using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
	private readonly IDistributedCache _redisCache;

	public BasketRepository(IDistributedCache redisCache)
	{
		_redisCache = redisCache;
	}

	public async Task<ShoppingCart?> GetBasket(string userName)
	{
		var basket = await _redisCache.GetAsync(userName);
		if (basket is null)
		{
			return null;
		}

		return await JsonSerializer.DeserializeAsync<ShoppingCart>(new MemoryStream(basket));
	}

	public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
	{
		await _redisCache.SetAsync(basket.UserName, JsonSerializer.SerializeToUtf8Bytes(basket));
		return (await GetBasket(basket.UserName))!;
	}

	public async Task DeleteBasket(string userName)
	{
		await _redisCache.RemoveAsync(userName);
	}
}