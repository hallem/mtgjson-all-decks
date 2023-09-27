using System.Text.Json;
using AllDecks.Helpers;

namespace AllDecks.Objects
{
    public class SourceProducts
	{
		public List<string> Foil { get; set; }
		public List<string> NonFoil { get; set; }

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

