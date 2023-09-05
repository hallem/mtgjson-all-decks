using System.Text.Json;
using All_Decks.Helpers;

namespace All_Decks.Objects
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

            return serializedJson;
        }
    }
}

