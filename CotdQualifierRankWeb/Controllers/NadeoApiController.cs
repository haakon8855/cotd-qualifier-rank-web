using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.DTOs;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CotdQualifierRankWeb.Controllers
{
    public class NadeoApiController
    {
        public static Dictionary<string, string> BaseURIs = new Dictionary<string, string>()
        {
            { "core", "https://prod.trackmania.core.nadeo.online/" },
            { "live", "https://live-services.trackmania.nadeo.live/" },
            { "meet", "https://meet.trackmania.nadeo.club/" },
        };

        static HttpClient _coreClient = new HttpClient();
        static HttpClient _liveClient = new HttpClient();
        static HttpClient _meetClient = new HttpClient();

        public string UserAgent { get; set; } = "";

        private static NadeoAuthTokens? AuthTokens = null;

        private DateTime LastRequest = DateTime.Now;

        public static readonly int RequestTimeoutInterval = 1000;

        private NadeoCredentialsManager _credentialsManager;

        public NadeoApiController(NadeoCredentialsManager credentialsManager)
        {
            _credentialsManager = credentialsManager;
            if (_credentialsManager.Credentials is null)
            {
                throw new InvalidOperationException("Credentials not found.");
            }
            Console.WriteLine("----------------------------------");
            Console.WriteLine(_credentialsManager.Credentials.Login);
            Console.WriteLine(_credentialsManager.Credentials.Password);
            Console.WriteLine(_credentialsManager.Credentials.AccountId);
            Console.WriteLine(_credentialsManager.Credentials.UserAgent);
            Console.WriteLine("----------------------------------");

            UserAgent = _credentialsManager.Credentials.UserAgent ?? "Cotd Qualifier Rank Web/1.0";
            _coreClient.BaseAddress = new Uri(BaseURIs["core"]);
            _liveClient.BaseAddress = new Uri(BaseURIs["live"]);
            _meetClient.BaseAddress = new Uri(BaseURIs["meet"]);
        }

        public async Task Authenticate()
        {
            if (_credentialsManager.Credentials is null
                || _credentialsManager.Credentials.Login is null
                || _credentialsManager.Credentials.Password is null)
            {
                throw new InvalidOperationException("Credentials not found.");
            }
            string username = _credentialsManager.Credentials.Login;
            string password = _credentialsManager.Credentials.Password;
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            _coreClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
            var jsonPayload = "{\"audience\":\"NadeoClubServices\"}";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "/v2/authentication/token/basic");
            request.Content = content;

            var response = await _coreClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    NadeoAuthTokens? tokens = JsonConvert.DeserializeObject<NadeoAuthTokens>(responseBody);
                    if (tokens is null)
                    {
                        return;
                    }
                    tokens.AuthTime = DateTime.Now;
                    AuthTokens = tokens;
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine($"Access token: {AuthTokens.AccessToken}");
                    Console.WriteLine($"Refresh token: {AuthTokens.RefreshToken}");
                    Console.WriteLine($"Auth time: {AuthTokens.AuthTime}");
                    Console.WriteLine("----------------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Throttle()
        {
            var timeToWait = LastRequest.AddMilliseconds(RequestTimeoutInterval) - DateTime.Now;
            if (timeToWait > TimeSpan.Zero)
            {
                Console.WriteLine($"Throttling for {timeToWait.TotalMilliseconds} ms");
                Thread.Sleep(timeToWait);
            }
            Console.WriteLine($"Time since last request: {DateTime.Now - LastRequest}");
            LastRequest = DateTime.Now;
        }

        public void SetDefaultRequestHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }

        public async Task SetAuthenticationHeaders(HttpClient client)
        {
            SetDefaultRequestHeaders(client);
            if (AuthTokens is null || AuthTokens.AuthTime is null || AuthTokens.AuthTime < DateTime.Now.AddHours(-12))
            {
                await Authenticate();
            }
            if (AuthTokens is not null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("nadeo_v1", $"t={AuthTokens.AccessToken}");
            }
        }

        public async Task<HttpResponseMessage?> GetTodtInfoForMap(string mapUid)
        {
            SetDefaultRequestHeaders(_liveClient);
            var endpointURI = $"/api/campaign/map/{mapUid}";
            try
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Sending request: {endpointURI}");
                Console.WriteLine("----------------------------------");
                Throttle();
                var response = await _liveClient.GetAsync(endpointURI);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public async Task<HttpResponseMessage?> GetCompetitions(int length, int offset)
        {
            await SetAuthenticationHeaders(_meetClient);
            var endpointURI = $"/api/competitions?length={length}&offset={offset}";
            try
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Sending request: {endpointURI}");
                Console.WriteLine("----------------------------------");
                Throttle();
                var response = await _meetClient.GetAsync(endpointURI);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public async Task<int> GetChallengeId(int competitionId)
        {
            await SetAuthenticationHeaders(_meetClient);
            var endpointURI = $"/api/competitions/{competitionId}/rounds";
            try
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Sending request: {endpointURI}");
                Console.WriteLine("----------------------------------");
                Throttle();
                var response = await _meetClient.GetAsync(endpointURI);
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var rounds = JsonConvert.DeserializeObject<List<NadeoCompetitionRoundsDTO>>(content);
                    Console.WriteLine(content);
                    if (rounds is null)
                    {
                        return 0;
                    }
                    var round = rounds.First();
                    if (round is not null)
                    {
                        return round.QualifierChallengeId;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public async Task<NadeoChallengeLeaderboardDTO?> GetLeaderboard(int challengeId, int length, int offset)
        {
            await SetAuthenticationHeaders(_meetClient);
            var endpointURI = $"/api/challenges/{challengeId}/leaderboard?length={length}&offset={offset}";
            try
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Sending request: {endpointURI}");
                Console.WriteLine("----------------------------------");
                Throttle();
                var response = await _meetClient.GetAsync(endpointURI);
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var leaderboard = JsonConvert.DeserializeObject<NadeoChallengeLeaderboardDTO>(content);
                    if (leaderboard is null)
                    {
                        return null;
                    }
                    return leaderboard;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}
