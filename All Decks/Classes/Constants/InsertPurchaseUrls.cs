// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string InsertPurchaseUrls =
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
}