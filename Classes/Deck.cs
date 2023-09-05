namespace All_Decks.Classes
{
	public class Deck
	{
		public Meta Meta { get; set; }
		public Data Data { get; set; }
	}

	public class Data
	{
		public string Code { get; set; }
		public List<CardDeck> Commander { get; set; }
		public string FileName { get; set; }
		public List<CardDeck> MainBoard { get; set; }
		public string Name { get; set; }
		public string ReleaseDate { get; set; }
		public List<CardDeck> SideBoard { get; set; }
		public string Type { get; set; }
	}
}

