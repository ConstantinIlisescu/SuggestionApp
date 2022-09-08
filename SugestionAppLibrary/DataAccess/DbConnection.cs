using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;
public class DbConnection
{

	private readonly IConfiguration _config;
	private readonly IMongoDatabase _db;
	private string _conectionId = "MongoDB";
	public string DbName { get; private set; }
	public string CategoryCollectionName { get; private set; } = "categories";
	public string StatusCollectionName { get; private set; } = "statuses";
	public string UserColectionName { get; private set; } = "users";
	public string SuggestionCollectionName { get; private set; } = "suggestions";


	public DbConnection(IConfiguration config)
	{
		_config = config;
	}
}
