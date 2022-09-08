
using SuggestionAppLibrary.Models;

namespace SuggestionAppLibrary.DataAccess;
public class MongoUserData
{
	private readonly IMongoCollection<UserModel> _users;
	public MongoUserData(IDbConnection db)
	{
		_users = db.UserCollection;
	}

	public async Task<List<UserModel>> GetUsersAsync()
	{
		var results = await _users.FindAsync(_ => true);
		return results.ToList();
	}

	public async Task<UserModel> GetUser(string id)
	{
		var results = await _users.FindAsync(u => u.Id == id);
		return results.FirstOrDefault();
	}
}
