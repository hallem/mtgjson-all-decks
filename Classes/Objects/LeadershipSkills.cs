using System.Text.Json;
using AllDecks.Helpers;

namespace AllDecks.Objects
{
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
}
