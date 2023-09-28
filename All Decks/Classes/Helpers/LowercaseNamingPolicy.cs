using System.Text.Json;

namespace All_Decks.Classes.Helpers;

public class LowercaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToLower();
}