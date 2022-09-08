using Microsoft.Extensions.Caching.Memory;
using SuggestionAppLibrary.Models;

namespace SuggestionAppLibrary.DataAccess;
public class MongoSuggestionData
{
	private readonly IDbConnection _db;
	private readonly IUserData _userData;
	private readonly IMemoryCache _cache;
	private readonly IMongoCollection<SuggestionModel> _suggestions;
	private const string CacheName = "SuggestionName";



	public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
	{
		_db = db;
		_userData = userData;
		_cache = cache;
		_suggestions = db.SuggestionCollection;
	}

	public async Task<List<SuggestionModel>> GetAllSuggestions()
	{
		var output = _cache.Get<List<SuggestionModel>>(CacheName);
		return output;
	}
}
