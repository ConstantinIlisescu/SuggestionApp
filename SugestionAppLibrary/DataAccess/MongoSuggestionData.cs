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

	public async Task<SuggestionModel> GetSuggestion(string id)
	{
		//This method goes to the server and gets all the suggestions that matches the Id passed
		var results = await _suggestions.FindAsync(s => s.Id == id);
		return results.FirstOrDefault();
	}

	public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
	{
		var output = await GetAllSuggestions();
		return output.Where(x =>
			x.ApprovedForRelease == false &&
			x.Rejected == false).ToList();
	}

	public async Task UpdateSuggestion(SuggestionModel suggestion)
	{
		await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
		_cache.Remove(CacheName);
	}

	public async Task UpvoteSuggestion(string suggestionId, string userId)
	{
		var client = _db.Client;

		using var session = await client.StartSessionAsync();

		session.StartTransaction();
		try
		{
			var db = client.GetDatabase(_db.DbName);
			var suggestionsInTransactions = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
			var suggestion = (await suggestionsInTransactions.FindAsync(s => s.Id == suggestionId)).First();

			bool isUpvote = suggestion.UserVotes.Add(userId);
			if (isUpvote == false)
			{
				suggestion.UserVotes.Remove(userId);
			}

			await suggestionsInTransactions.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);

			var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
			var user = await _userData.GetUser(suggestion.Author.Id);

			if (isUpvote)
			{
				user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
			}
			else
			{
				var suggestionToRemove = user.VotedOnSuggestions.Where(s => s.Id == suggestionId).First();
				user.VotedOnSuggestions.Remove(suggestionToRemove);
			}

			await usersInTransaction.ReplaceOneAsync(u => u.Id == userId, user);

			await session.CommitTransactionAsync();

			_cache.Remove(CacheName);
		}
		catch (Exception ex)
		{
			await session.AbortTransactionAsync();
			throw;
		}

	}

	public async Task CreateSuggestion(SuggestionModel suggestion)
	{
		var client = _db.Client;

		using var session = await client.StartSessionAsync();

		session.StartTransaction();

		try
		{
			var db = client.GetDatabase(_db.DbName);
			var suggestionsInTransactions = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
			await suggestionsInTransactions.InsertOneAsync(suggestion);

			var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
			var user = await _userData.GetUser(suggestion.Author.Id);
			user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
			await usersInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

			await session.CommitTransactionAsync();
		}
		catch (Exception ex)
		{
			await session.AbortTransactionAsync();
			throw;
		}
	}
}
