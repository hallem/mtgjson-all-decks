using AllDecks.Objects;

namespace AllDecks.Classes
{
	public class Deck
	{
		public Meta Meta { get; set; }
		public Data Data { get; set; }
	}

	public class DeckSlim
	{
		public Meta Meta { get; set; }
		public DataSlim Data { get; set; }
	}

    public class DataCommon
    {
        public string Code { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public string ReleaseDate { get; set; }
        public string Type { get; set; }
    }

    public class Data : DataCommon
    {
        public List<CardDeck> Commander { get; set; }
        public List<CardDeck> MainBoard { get; set; }
        public List<CardDeck> SideBoard { get; set; }
    }
    public class DataSlim : DataCommon
    {
        public List<CardDeckSlim> Commander { get; set; }
        public List<CardDeckSlim> MainBoard { get; set; }
        public List<CardDeckSlim> SideBoard { get; set; }
    }
}

