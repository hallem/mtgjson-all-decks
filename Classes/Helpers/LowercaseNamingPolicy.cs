using System.Text.Json;

namespace AllDecks.Helpers
{
    public class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}

