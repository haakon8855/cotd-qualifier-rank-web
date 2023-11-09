using CotdQualifierRankWeb.Models;

namespace CotdQualifierRankWeb.Data
{
    public class NadeoCredentialsManager
    {
        private readonly IConfiguration _configuration;
        public Credentials? Credentials { get; set; }

        public NadeoCredentialsManager(IConfiguration configuration)
        {
            _configuration = configuration;

            Console.WriteLine("starting");
            var accountId = _configuration["AzureKeyVault:nadeo-accountid"];
            var login = _configuration["AzureKeyVault:nadeo-login"];
            var password = _configuration["AzureKeyVault:nadeo-password"];
            var useragent = _configuration["AzureKeyVault:nadeo-useragent"];

            Credentials = new Credentials
            {
                AccountId = accountId,
                Login = login,
                Password = password,
                UserAgent = useragent
            };
            Console.WriteLine(accountId);
            Console.WriteLine(login);
            Console.WriteLine(password);
            Console.WriteLine(useragent);
        }
    }
}
