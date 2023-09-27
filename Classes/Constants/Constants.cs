namespace AllDecks.Constants
{
	// ReSharper disable UseRawString
	public static class Constants
	{
		public const string CreateDecksStatement = 
@"CREATE TABLE IF NOT EXISTS ""decks"" (
	""code"" text NOT NULL,
	""fileName"" text NOT NULL,
	""name"" text NOT NULL,
	""releaseDate"" text,
	""type"" text NOT NULL
);

CREATE INDEX IF NOT EXISTS decks_fileName ON ""decks"" (""fileName"");";

		public const string InsertDecksStatement =
@"INSERT INTO decks (
	""code"",
	""fileName"",
	""name"",
	""releaseDate"",
	""type""
) VALUES (
";

		public const string CreateDeckListsStatement =
@"CREATE TABLE IF NOT EXISTS ""deckLists"" (
	""count"" integer,
	""fileName"" text,
	""isCommander"" boolean,
	""isFoil"" boolean,
	""isMainBoard"" boolean,
	""isSideBoard"" boolean,
	""uuid"" varchar(36) NOT NULL
);

CREATE INDEX IF NOT EXISTS deckLists_fileName ON ""deckLists"" (""fileName"");

CREATE INDEX IF NOT EXISTS deckLists_uuid ON ""deckLists"" (""uuid"");";

        public const string InsertDeckListsStatement =
@"INSERT INTO deckLists (
	""count"",
	""fileName"",
	""isCommander"",
	""isFoil"",
	""isMainBoard"",
	""isSideBoard"",
	""uuid""
) VALUES (
";

		public const string InsertCardsStatement =
@"INSERT INTO cards (
	""artist"",
	""artistIds"",
	""asciiName"",
	""attractionLights"",
	""availability"",
	""boosterTypes"",
	""borderColor"",
	""cardParts"",
	""colorIdentity"",
	""colorIndicator"",
	""colors"",
	""count"",
	""defense"",
	""duelDeck"",
	""edhrecRank"",
	""edhrecSaltiness"",
	""faceConvertedManaCost"",
	""faceFlavorName"",
	""faceManaValue"",
	""faceName"",
	""fileName"",
	""finishes"",
	""flavorName"",
	""flavorText"",
	""frameEffects"",
	""frameVersion"",
	""hand"",
	""hasAlternativeDeckLimit"",
	""hasContentWarning"",
	""hasFoil"",
	""hasNonFoil"",
	""isAlternative"",
	""isCommander"",
	""isFoil"",
	""isFullArt"",
	""isFunny"",
	""isMainBoard"",
	""isOnlineOnly"",
	""isOversized"",
	""isPromo"",
	""isRebalanced"",
	""isReprint"",
	""isReserved"",
	""isSideBoard"",
	""isStarter"",
	""isStorySpotlight"",
	""isTextless"",
	""isTimeshifted"",
	""keywords"",
	""language"",
	""layout"",
	""leadershipSkills"",
	""life"",
	""loyalty"",
	""manaCost"",
	""manaValue"",
	""name"",
	""number"",
	""originalPrintings"",
	""originalReleaseDate"",
	""originalText"",
	""originalType"",
	""otherFaceIds"",
	""power"",
	""printings"",
	""promoTypes"",
	""rarity"",
	""rebalancedPrintings"",
	""relatedCards"",
	""securityStamp"",
	""setCode"",
	""side"",
	""signature"",
	""sourceProducts"",
	""subsets"",
	""subtypes"",
	""supertypes"",
	""text"",
	""toughness"",
	""type"",
	""types"",
	""uuid"",
	""variations"",
	""watermark""
) VALUES (
";

		public const string InsertForeignDataStatement =
@"INSERT INTO cardForeignData (
	""faceName"",
	""flavorText"",
	""language"",
	""multiverseId"",
	""name"",
	""text"",
	""type"",
	""uuid""
) VALUES (
";
		public const string InsertIdentifiersStatement =
@"INSERT INTO cardIdentifiers (
	""cardKingdomEtchedId"",
	""cardKingdomFoilId"",
	""cardKingdomId"",
	""cardsphereId"",
	""mcmId"",
	""mcmMetaId"",
	""mtgArenaId"",
	""mtgjsonFoilVersionId"",
	""mtgjsonNonFoilVersionId"",
	""mtgjsonV4Id"",
	""mtgoFoilId"",
	""mtgoId"",
	""multiverseId"",
	""scryfallId"",
	""scryfallIllustrationId"",
	""scryfallOracleId"",
	""tcgplayerEtchedProductId"",
	""tcgplayerProductId"",
	""uuid""
) VALUES (
";

		public const string InsertLegalitiesStatement =
@"INSERT INTO cardLegalities (
	""alchemy"",
	""brawl"",
	""commander"",
	""duel"",
	""explorer"",
	""future"",
	""gladiator"",
	""historic"",
	""historicbrawl"",
	""legacy"",
	""modern"",
	""oathbreaker"",
	""oldschool"",
	""pauper"",
	""paupercommander"",
	""penny"",
	""pioneer"",
	""predh"",
	""premodern"",
	""standard"",
	""uuid"",
	""vintage""
) VALUES (
";
		public const string InsertPurchaseUrlsStatement =
@"INSERT INTO cardPurchaseUrls (
	""cardKingdom"",
	""cardKingdomEtched"",
	""cardKingdomFoil"",
	""cardmarket"",
	""tcgplayer"",
	""tcgplayerEtched"",
	""uuid""
) VALUES (
";

		public const string InsertRulingsStatement =
@"INSERT INTO cardRulings (
	""date"",
	""text"",
	""uuid""
) VALUES (
";
	}
}

