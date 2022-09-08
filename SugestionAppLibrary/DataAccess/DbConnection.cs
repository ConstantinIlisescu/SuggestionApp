using Microsoft.Extensions.Configuration;

namespace SuggestionAppLibrary.DataAccess;
public class DbConnection
{

	private readonly IConfiguration _config;

	public DbConnection(IConfiguration config)
	{
		_config = config;
	}
}
