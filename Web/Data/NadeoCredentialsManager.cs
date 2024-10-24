using CotdQualifierRank.Web.Models;

namespace CotdQualifierRank.Web.Data;

public class NadeoCredentialsManager
{
    private readonly IConfiguration _configuration;
    public Credentials? Credentials { get; set; }

    public NadeoCredentialsManager(IConfiguration configuration)
    {
        _configuration = configuration;

        Console.WriteLine("starting");
        var accountId = _configuration["nadeo-accountid"];
        var login = _configuration["nadeo-login"];
        var password = _configuration["nadeo-password"];
        var useragent = _configuration["nadeo-useragent"];
        var connectionstring = _configuration["database-connection-string"];

        Credentials = new Credentials
        {
            AccountId = accountId,
            Login = login,
            Password = password,
            UserAgent = useragent
        };

        Console.WriteLine(connectionstring);
        Console.WriteLine(accountId);
        Console.WriteLine(login);
        Console.WriteLine(password);
        Console.WriteLine(useragent);
    }
}

