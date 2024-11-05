using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using CotdQualifierRank.Domain.DomainPrimitives;
using CotdQualifierRank.Web.Data;
using CotdQualifierRank.Web.DTOs;

namespace CotdQualifierRank.Web.Services;

public class NadeoApiService
{
    private static readonly Dictionary<string, string> BaseURIs = new()
    {
        { "core", "https://prod.trackmania.core.nadeo.online/" },
        { "live", "https://live-services.trackmania.nadeo.live/" },
        { "meet", "https://meet.trackmania.nadeo.club/" },
    };

    private static readonly HttpClient CoreClient = new();
    private static readonly HttpClient LiveClient = new();
    private static readonly HttpClient MeetClient = new();
    private static NadeoAuthTokens? _authTokens;
    private DateTime _lastRequest = DateTime.Now;
    private readonly NadeoCredentialsManager _credentialsManager;
    private readonly string _userAgent;
    private const int RequestTimeoutInterval = 1000;

    public NadeoApiService(NadeoCredentialsManager credentialsManager)
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

        _userAgent = _credentialsManager.Credentials.UserAgent ?? "Cotd Qualifier Rank Web/1.0";
        CoreClient.BaseAddress = new Uri(BaseURIs["core"]);
        LiveClient.BaseAddress = new Uri(BaseURIs["live"]);
        MeetClient.BaseAddress = new Uri(BaseURIs["meet"]);
    }

    private async Task Authenticate()
    {
        if (_credentialsManager.Credentials is null
            || _credentialsManager.Credentials.Login is null
            || _credentialsManager.Credentials.Password is null)
        {
            throw new InvalidOperationException("Credentials not found.");
        }

        var username = _credentialsManager.Credentials.Login;
        var password = _credentialsManager.Credentials.Password;
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        CoreClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
        var jsonPayload = "{\"audience\":\"NadeoClubServices\"}";
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "/v2/authentication/token/basic");
        request.Content = content;

        var response = await CoreClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                var tokens = JsonConvert.DeserializeObject<NadeoAuthTokens>(responseBody);
                if (tokens is null)
                    return;

                tokens.AuthTime = DateTime.Now;
                _authTokens = tokens;
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Access token: {_authTokens.AccessToken}");
                Console.WriteLine($"Refresh token: {_authTokens.RefreshToken}");
                Console.WriteLine($"Auth time: {_authTokens.AuthTime}");
                Console.WriteLine("----------------------------------");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    private void Throttle()
    {
        var timeToWait = _lastRequest.AddMilliseconds(RequestTimeoutInterval) - DateTime.Now;
        if (timeToWait > TimeSpan.Zero)
        {
            Console.WriteLine($"Throttling for {timeToWait.TotalMilliseconds} ms");
            Thread.Sleep(timeToWait);
        }

        Console.WriteLine($"Time since last request: {DateTime.Now - _lastRequest}");
        _lastRequest = DateTime.Now;
    }

    private void SetDefaultRequestHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
    }

    private async Task SetAuthenticationHeaders(HttpClient client)
    {
        SetDefaultRequestHeaders(client);
        if (_authTokens?.AuthTime is null || _authTokens.AuthTime < DateTime.Now.AddHours(-12))
            await Authenticate();

        if (_authTokens is not null)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("nadeo_v1", $"t={_authTokens.AccessToken}");
        }
    }

    public async Task<HttpResponseMessage?> GetTodtInfoForMap(MapUid mapUid)
    {
        SetDefaultRequestHeaders(LiveClient);
        var endpointURI = $"/api/campaign/map/{mapUid}";
        try
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Sending request: {endpointURI}");
            Console.WriteLine("----------------------------------");
            Throttle();
            var response = await LiveClient.GetAsync(endpointURI);
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
        await SetAuthenticationHeaders(MeetClient);
        var endpointURI = $"/api/competitions?length={length}&offset={offset}";
        try
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Sending request: {endpointURI}");
            Console.WriteLine("----------------------------------");
            Throttle();
            var response = await MeetClient.GetAsync(endpointURI);
            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }

    public async Task<int> GetChallengeId(NadeoCompetitionId competitionId)
    {
        await SetAuthenticationHeaders(MeetClient);
        var endpointURI = $"/api/competitions/{competitionId}/rounds";
        try
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Sending request: {endpointURI}");
            Console.WriteLine("----------------------------------");
            Throttle();
            var response = await MeetClient.GetAsync(endpointURI);
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var rounds = JsonConvert.DeserializeObject<List<NadeoCompetitionRoundsDTO>>(content);
                Console.WriteLine(content);
                if (rounds is null)
                {
                    return 0;
                }

                var round = rounds.FirstOrDefault();
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

    public async Task<NadeoChallengeLeaderboardDTO?> GetLeaderboard(NadeoChallengeId challengeId, int length, int offset)
    {
        await SetAuthenticationHeaders(MeetClient);
        var endpointURI = $"/api/challenges/{challengeId}/leaderboard?length={length}&offset={offset}";
        try
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Sending request: {endpointURI}");
            Console.WriteLine("----------------------------------");
            Throttle();
            var response = await MeetClient.GetAsync(endpointURI);
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var leaderboard = JsonConvert.DeserializeObject<NadeoChallengeLeaderboardDTO>(content);
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