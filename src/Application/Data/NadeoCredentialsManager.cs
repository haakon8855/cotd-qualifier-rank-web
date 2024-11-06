using CotdQualifierRank.Database.Models;
using Microsoft.Extensions.Configuration;

namespace CotdQualifierRank.Application.Data;

public class NadeoCredentialsManager
{
    private readonly IConfiguration _configuration;
    public Credentials? Credentials { get; set; }

    public NadeoCredentialsManager(IConfiguration configuration)
    {
        _configuration = configuration;

        var accountId = _configuration["nadeo-accountid"];
        var login = _configuration["nadeo-login"];
        var password = _configuration["nadeo-password"];
        var useragent = _configuration["nadeo-useragent"];

        Credentials = new Credentials
        {
            AccountId = accountId,
            Login = login,
            Password = password,
            UserAgent = useragent
        };
    }
}