// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreateIdentifiers = 
        @"CREATE TABLE IF NOT EXISTS ""cardIdentifiers"" (
  ""cardKingdomEtchedId"" text,
  ""cardKingdomFoilId"" text,
  ""cardKingdomId"" text,
  ""cardsphereId"" text,
  ""mcmId"" text,
  ""mcmMetaId"" text,
  ""mtgArenaId"" text,
  ""mtgjsonFoilVersionId"" text,
  ""mtgjsonNonFoilVersionId"" text,
  ""mtgjsonV4Id"" text,
  ""mtgoFoilId"" text,
  ""mtgoId"" text,
  ""multiverseId"" text,
  ""scryfallId"" text,
  ""scryfallIllustrationId"" text,
  ""scryfallOracleId"" text,
  ""tcgplayerEtchedProductId"" text,
  ""tcgplayerProductId"" text,
  ""uuid"" text
);

CREATE INDEX IF NOT EXISTS cardIdentifiers_uuid ON ""cardIdentifiers"" (""uuid"");";
}