using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
	private readonly ICatalogContext _catalogContext;

	public ProductRepository(ICatalogContext catalogContext)
	{
		_catalogContext = catalogContext;
	}
	public async Task<IEnumerable<Product>> GetProducts()
	{
		return await _catalogContext.Products.Find(p => true).ToListAsync();

	}

	
	public async Task<Product?> GetProduct(string id)
	{
		var p = await _catalogContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
		return p;
	}

	public async Task<IEnumerable<Product>> GetProductByName(string name)
	{
		return await _catalogContext.Products.Find(p => p.Name == name).ToListAsync();
	}

	public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
	{
		return await _catalogContext.Products.Find(p => p.Category == categoryName).ToListAsync();
	}

	public async Task CreateProduct(Product product)
	{
		await _catalogContext.Products.InsertOneAsync(product);
	}

	public async Task<bool> UpdateProduct(Product product)
	{
		var res = await _catalogContext.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
		return res.IsAcknowledged && res.ModifiedCount > 0;
	}

	public async Task<bool> DeleteProduct(string id)
	{ 
		var res = await _catalogContext.Products.DeleteOneAsync(p => p.Id == id);
		return res.IsAcknowledged && res.DeletedCount > 0;
	}
}