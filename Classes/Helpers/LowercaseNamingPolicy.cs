using System.Text.Json;

namespace All_Decks.Helpers
{
    public class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}

