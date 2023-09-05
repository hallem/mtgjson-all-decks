using System.Text.Json;
using All_Decks.Helpers;

namespace All_Decks.Objects
{
	public class RelatedCards
	{
        public string ReverseRelated { get; set; }
		public string Spellbook { get; set; }

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
