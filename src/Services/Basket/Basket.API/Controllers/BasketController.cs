using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Basket.API.Controllers;
[Route("api/Basket")]
[ApiController]
public class BasketController : ControllerBase
{
	private readonly IBasketRepository _repository;
	private readonly IConfiguration _configuration;
	private readonly ILogger<BasketController> _logger;

	public BasketController(
		IBasketRepository repository, 
		IConfiguration configuration, 
		ILogger<BasketController> logger)
	{
		_repository = repository;
		_configuration = configuration;
		_logger = logger;
	}

	[HttpGet("{userName}", Name = nameof(GetBasket))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
	public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
	{
		var basket = await _repository.GetBasket(userName);
		return Ok(basket ?? new ShoppingCart(userName));
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ShoppingCart))]
	public async Task<ActionResult<ShoppingCart>> UpdateShoppingCart(ShoppingCart basket)
	{
		return Ok(await _repository.UpdateBasket(basket)); 
	}

	[HttpDelete]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult> DeleteShoppingCart(string userName)
	{
		await _repository.DeleteBasket(userName);
		return Ok();
	}
}
