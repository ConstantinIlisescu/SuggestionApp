﻿using Microsoft.Extensions.Caching.Memory;
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

		// saves whatever is in cache saved under variable CacheName
		var output = _cache.Get<List<SuggestionModel>>(CacheName);
		//checks the output
		if (output == null)
		{
			//looks into teh database and get all the data dat is not archived
			var results = await _suggestions.FindAsync(s => s.Archived == false);
			//stores it as a list
			output = results.ToList();
			//caches it for 1 minute
			_cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
		}
		return output;
	}

	public async Task<List<SuggestionModel>> GetAllApprovedSuggestions()
	{
		var output = await GetAllApprovedSuggestions();
		return output.Where(x => x.ApprovedForRelease).ToList();
	}


}
