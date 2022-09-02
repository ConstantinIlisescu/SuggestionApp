namespace SuggestionAppLibrary.Models;
public class BasicSuggestionModel
{
   [BsonRepresentation(BsonType.ObjectId)]
   public string Id { get; set; }
   public string Title { get; set; }
}
