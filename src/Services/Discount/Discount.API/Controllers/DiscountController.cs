using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Discount.API.Controllers;
[Route("api/Discount")]
[ApiController]
public class DiscountController : ControllerBase
{
	private readonly IDiscountRepository _repository;
	private readonly ILogger<DiscountController> _logger;

	public DiscountController(IDiscountRepository repository, ILogger<DiscountController> logger)
	{
		_repository = repository;
		_logger = logger;
	}
	[HttpGet("{productName}",Name= nameof(GetDiscount))]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<Coupon>> GetDiscount(string productName)
	{
		var discount =await _repository.GetDiscount(productName);
		if (discount == null)
		{
			return NotFound($"Product {productName} not found.");
		}
		return Ok(discount);
	}


	// POST api/<DiscountController>
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
	{
		var res = await _repository.CreateDiscount(coupon);
		if (res == false)
			return BadRequest($"Error while trying to create discount {coupon.ProductName}.");
		return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
	}

	// PUT api/<DiscountController>/5
	[HttpPut]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
	{
		var res  = await _repository.UpdateDiscount(coupon);
		if (res == false) return BadRequest($"Error while trying to update discount {coupon.ProductName}.");
		return NoContent();
	}

	// DELETE api/<DiscountController>/5
	[HttpDelete("{productName}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<Coupon>> Delete(string productName)
	{
		var res = await _repository.DeleteDiscount(productName);
		return Ok();
	}
}
