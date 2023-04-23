using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data;

public class CatalogContext : ICatalogContext
{
	private readonly IConfiguration _configuration;

	public CatalogContext(IConfiguration configuration)
	{
		_configuration = configuration;
		MongoClient client = new MongoClient(_configuration["DatabaseSettings:ConnectionString"]);
		IMongoDatabase? database = client.GetDatabase(_configuration["DatabaseSettings:DatabaseName"]);
		Products = database.GetCollection<Product>(_configuration["DatabaseSettings:CollectionName"]);
		CatalogContextSeed.SeedData(Products);
	}
	public IMongoCollection<Product> Products { get; }
}