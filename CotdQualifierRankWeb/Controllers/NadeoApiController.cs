using CotdQualifierRankWeb.Data;

namespace CotdQualifierRankWeb.Controllers
{
    public class NadeoApiController
    {
        static HttpClient _liveClient = new HttpClient();
        static HttpClient _meetClient = new HttpClient();

        private CredentialsManager _credentialsManager;

        public static Dictionary<string, string> BaseURIs = new Dictionary<string, string>()
        {
            { "core", "https://prod.trackmania.core.nadeo.online/" },
            { "live", "https://live-services.trackmania.nadeo.live/" },
            { "meet", "https://meet.trackmania.nadeo.club/" },
        };

        public NadeoApiController(CredentialsManager credentialsManager)
        {
            _credentialsManager = credentialsManager;
            if (_credentialsManager.Credentials == null)
            {
                throw new InvalidOperationException("Credentials not found.");
            }
            Console.WriteLine("----------------------------------");
            Console.WriteLine(_credentialsManager.Credentials.Login);
            Console.WriteLine(_credentialsManager.Credentials.Password);
            Console.WriteLine(_credentialsManager.Credentials.AccoundId);
            Console.WriteLine("----------------------------------");

            _liveClient.BaseAddress = new Uri(BaseURIs["live"]);
            _meetClient.BaseAddress = new Uri(BaseURIs["meet"]);
        }

        public async Task<HttpResponseMessage?> GetTodtInfoFromMap(string mapUid)
        {
            _liveClient.DefaultRequestHeaders.Accept.Clear();
            var endpointURI = $"/api/campaign/map/{mapUid}";
            try
            {
                var response = await _liveClient.GetAsync(endpointURI);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
