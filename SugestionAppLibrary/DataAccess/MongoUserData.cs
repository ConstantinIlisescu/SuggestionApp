
using SuggestionAppLibrary.Models;

namespace SuggestionAppLibrary.DataAccess;
public class MongoUserData
{
	private readonly IMongoCollection<UserModel> _users;
	public MongoUserData(IDbConnection db)
	{
		_users = db.UserCollection;
	}

}
