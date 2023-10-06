using CotdQualifierRankWeb.Models;
using System.Text.Json;

namespace CotdQualifierRankWeb.Data
{
    public class CredentialsManager
    {
        private readonly string _jsonPath = "credentials.json";
        public Credentials? Credentials { get; set; }

        public CredentialsManager()
        {
            Credentials = JsonSerializer.Deserialize<Credentials>(File.ReadAllText(_jsonPath));
            if (Credentials == null)
            {
                throw new InvalidOperationException("Credentials not found.");
            }
        }
    }
}
