namespace All_Decks.Classes
{
	public class DeckList
	{
		public string UUID { get; set; }
		public string Name { get; set; }
		public string FileName { get; set; }
		public int Count { get; set; }
		public bool IsFoil { get; set; }
		public bool IsCommander { get; set; }
		public bool IsMainBoard { get; set; }
		public bool IsSideboard { get; set; }
	}
}

