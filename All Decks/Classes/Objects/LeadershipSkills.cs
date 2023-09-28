using System.Text.Json;
using All_Decks.Classes.Helpers;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace All_Decks.Classes.Objects;

public class LeadershipSkills
{
    public bool Brawl { get; set; }
    public bool Commander { get; set; }
    public bool Oathbreaker { get; set; }

    public override string ToString()
    {
        string serializedJson = JsonSerializer.Serialize(this,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowercaseNamingPolicy(),
                WriteIndented = false
            });

        return serializedJson.Replace("\"", "\"\"");
    }
}