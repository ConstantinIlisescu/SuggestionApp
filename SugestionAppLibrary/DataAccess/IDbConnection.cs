using MongoDB.Driver;
using SuggestionAppLibrary.Models;

namespace SuggestionAppLibrary.DataAccess;
public interface IDbConnection
{
   IMongoCollection<CategoryModel> CategoryCollection { get; }
   string CategoryCollectionName { get; }
   MongoClient Client { get; }
   string DbName { get; }
   IMongoCollection<StatusModel> StatusCollection { get; }
   string StatusCollectionName { get; }
   IMongoCollection<SuggestionModel> SuggestionCollection { get; }
   string SuggestionCollectionName { get; }
   string UserColectionName { get; }
   IMongoCollection<UserModel> UserCollection { get; }
}