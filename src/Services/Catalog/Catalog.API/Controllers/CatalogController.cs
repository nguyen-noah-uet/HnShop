using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;
[Route("api/Catalog")]
[ApiController]
public class CatalogController : ControllerBase
{
	private readonly IProductRepository _repository;
	private readonly ILogger<CatalogController> _logger;

	public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
	{
		_repository = repository;
		_logger = logger;
	}
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
	{
		return Ok(await _repository.GetProducts());
	}

	[HttpGet("{id:length(24)}", Name = "GetProduct")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
	public async Task<ActionResult<Product>> GetProductById(string id)
	{
		Product? product = await _repository.GetProduct(id);
		if (product is null)
		{
			_logger.LogError($"Product with id: {id}, not found.");
			return NotFound();
		}
		return Ok(product);
	}

	[HttpGet("[action]/{categoryName}", Name = "GetProductByCategory")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string categoryName)
	{
		IEnumerable<Product> products = await _repository.GetProductByCategory(categoryName);
		if (!products.Any())
		{
			_logger.LogError($"Product with category: {categoryName}, not found.");
			return NotFound();
		}
		return Ok(products);
	}

	[HttpGet("[action]/{name}", Name = "GetProductByName")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name)
	{
		IEnumerable<Product> products = await _repository.GetProductByName(name);
		if (!products.Any())
		{
			_logger.LogError($"Product with name: {name}, not found.");
			return NotFound();
		}
		return Ok(products);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
	{
		await _repository.CreateProduct(product);
		return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
	}

	[HttpPut(Name = "UpdateProduct")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UpdateProduct([FromBody] Product product)
	{
		return Ok(await _repository.UpdateProduct(product));
	}

	[HttpDelete("{id:length(24)}",Name = "DeleteProduct")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> DeleteProductById(string id)
	{
		return Ok(await _repository.DeleteProduct(id));
	}
}
