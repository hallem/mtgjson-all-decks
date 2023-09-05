using System.Text.Json;

namespace All_Decks.Classes
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

    public class LowercaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}

