using System.Text.Json;
using AllDecks.Helpers;

namespace AllDecks.Objects
{
	public class RelatedCards
	{
        public List<string> ReverseRelated { get; set; }
		public List<string> Spellbook { get; set; }

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
