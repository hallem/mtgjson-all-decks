using All_Decks.Helpers;

namespace All_Decks.Objects
{
    public class CardDeck
	{
		public string Artist { get; set; }
		public CsvList<string> ArtistIds { get; set; }
		public string AsciiName { get; set; } // doesn't seem to be included in the deck files
		public string AttractionLights { get; set; } // doesn't seem to be included in the deck files
		public CsvList<string> Availability { get; set; }
		public CsvList<string> BoosterTypes { get; set; } // doesn't seem to be included in the deck files
		public string BorderColor { get; set; }
		public CsvList<string> CardParts { get; set; }
		public CsvList<string> ColorIdentity { get; set; }
		public string ColorIndicator { get; set; }
		public CsvList<string> Colors { get; set; }
		public float ConvertedManaCost { get; set; } // Not in DB
		public int Count { get; set; } // Added for Card Deck
		public int? Defense { get; set; }
		public string DuelDeck { get; set; }
		public int? EdhrecRank { get; set; }
		public float? EdhrecSaltiness { get; set; }
		public float? FaceConvertedManaCost { get; set; }
		public string FaceFlavorName { get; set; }
		public int? FaceManaValue { get; set; }
		public string FaceName { get; set; }
		public string FileName { get; set; } // Added for Card Deck, Primary Key (fileName, UUID), Foreign Key
		public CsvList<string> Finishes { get; set; }
		public string FlavorName { get; set; }
		public string FlavorText { get; set; }
		public List<ForeignData> ForeignData { get; set; }
        public CsvList<string> FrameEffects { get; set; }
		public string FrameVersion { get; set; }
		public string Hand { get; set; }
		public string HasAlternativeDeckLimit { get; set; }
		public string HasContentWarning { get; set; }
		public bool HasFoil { get; set; }
		public bool HasNonFoil { get; set; }
		public Identifiers Identifiers { get; set; }
        public bool? IsAlternative { get; set; }
		public bool IsCommander { get; set; } // Added for Card Deck
		public bool IsFoil { get; set; } // Added for Card Deck
		public bool? IsFullArt { get; set; }
		public bool? IsFunny { get; set; }
		public bool IsMainBoard { get; set; } // Added for Card Deck
		public bool? IsOnlineOnly { get; set; }
		public bool? IsOversized { get; set; }
		public bool? IsPromo { get; set; }
		public bool? IsRebalanced { get; set; }
		public bool? IsReprint { get; set; }
		public bool? IsReserved { get; set; }
		public bool IsSideBoard { get; set; }
		public bool? IsStarter { get; set; }
		public bool? IsStorySpotlight { get; set; }
		public bool? IsTextless { get; set; }
		public bool? IsTimeshifted { get; set; }
		public CsvList<string> Keywords { get; set; }
		public string Language { get; set; }
		public string Layout { get; set; }
		public LeadershipSkills LeadershipSkills { get; set; } // Serialized Json
        public Legalities Legalities { get; set; }
        public string Life { get; set; }
		public string Loyalty { get; set; }
		public string ManaCost { get; set; }
		public float ManaValue { get; set; }
		public string Name { get; set; }
		public string Number { get; set; }
		public CsvList<string> OriginalPrintings { get; set; }
		public string OriginalReleaseDate { get; set; }
		public string OriginalText { get; set; }
		public string OriginalType { get; set; }
		public CsvList<string> OtherFaceIds { get; set; }
		public string Power { get; set; }
		public CsvList<string> Printings { get; set; }
		public CsvList<string> PromoTypes { get; set; }
		public PurchaseUrls PurchaseUrls { get; set; }
        public string Rarity { get; set; }
        public CsvList<string> RebalancedPrintings { get; set; }
        public RelatedCards RelatedCards { get; set; } // Serialized Json
		public List<Rulings> Rulings { get; set; }
        public string SecurityStamp { get; set; }
		public string SetCode { get; set; }
		public string Side { get; set; }
		public string Signature { get; set; }
		public SourceProducts SourceProducts { get; set; } // Serialized Json
		public CsvList<string> Subsets { get; set; }
		public CsvList<string> Subtypes { get; set; }
		public CsvList<string> Supertypes { get; set; }
		public string Text { get; set; }
		public string Toughness { get; set; }
		public string Type { get; set; }
		public CsvList<string> Types { get; set; }
		public string UUID { get; set; } // Primary Key (FileName, UUID)
		public CsvList<string> Variations { get; set; }
		public string Watermark { get; set; }
	}
}
