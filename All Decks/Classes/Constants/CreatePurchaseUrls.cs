// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreatePurchaseUrls = @"CREATE TABLE IF NOT EXISTS ""cardPurchaseUrls"" (
  ""cardKingdom"" text,
  ""cardKingdomEtched"" text,
  ""cardKingdomFoil"" text,
  ""cardmarket"" text,
  ""tcgplayer"" text,
  ""tcgplayerEtched"" text,
  ""uuid"" text
);

CREATE INDEX IF NOT EXISTS cardPurchaseUrls_uuid ON ""cardPurchaseUrls"" (""uuid"");";
}