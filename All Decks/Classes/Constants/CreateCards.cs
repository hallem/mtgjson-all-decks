// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreateCards = 
        @"CREATE TABLE IF NOT EXISTS ""cards"" (
  ""artist"" text,
  ""artistIds"" text,
  ""asciiName"" text,
  ""attractionLights"" text,
  ""availability"" text,
  ""boosterTypes"" text,
  ""borderColor"" text,
  ""cardParts"" text,
  ""colorIdentity"" text,
  ""colorIndicator"" text,
  ""colors"" text,
  ""count"" integer,
  ""defense"" text,
  ""duelDeck"" text,
  ""edhrecRank"" integer,
  ""edhrecSaltiness"" float,
  ""faceConvertedManaCost"" float,
  ""faceFlavorName"" text,
  ""faceManaValue"" float,
  ""faceName"" text,
  ""fileName"" text,
  ""finishes"" text,
  ""flavorName"" text,
  ""flavorText"" text,
  ""frameEffects"" text,
  ""frameVersion"" text,
  ""hand"" text,
  ""hasAlternativeDeckLimit"" boolean,
  ""hasContentWarning"" boolean,
  ""hasFoil"" boolean,
  ""hasNonFoil"" boolean,
  ""isAlternative"" boolean,
  ""isCommander"" boolean,
  ""isFoil"" boolean,
  ""isFullArt"" boolean,
  ""isFunny"" boolean,
  ""isMainBoard"" boolean,
  ""isOnlineOnly"" boolean,
  ""isOversized"" boolean,
  ""isPromo"" boolean,
  ""isRebalanced"" boolean,
  ""isReprint"" boolean,
  ""isReserved"" boolean,
  ""isStarter"" boolean,
  ""isSideBoard"" boolean,
  ""isStorySpotlight"" boolean,
  ""isTextless"" boolean,
  ""isTimeshifted"" boolean,
  ""keywords"" text,
  ""language"" text,
  ""layout"" text,
  ""leadershipSkills"" text,
  ""life"" text,
  ""loyalty"" text,
  ""manaCost"" text,
  ""manaValue"" float,
  ""name"" text,
  ""number"" text,
  ""originalPrintings"" text,
  ""originalReleaseDate"" text,
  ""originalText"" text,
  ""originalType"" text,
  ""otherFaceIds"" text,
  ""power"" text,
  ""printings"" text,
  ""promoTypes"" text,
  ""rarity"" text,
  ""rebalancedPrintings"" text,
  ""relatedCards"" text,
  ""securityStamp"" text,
  ""setCode"" text,
  ""side"" text,
  ""signature"" text,
  ""sourceProducts"" text,
  ""subsets"" text,
  ""subtypes"" text,
  ""supertypes"" text,
  ""text"" text,
  ""toughness"" text,
  ""type"" text,
  ""types"" text,
  ""uuid"" varchar(36) NOT NULL,
  ""variations"" text,
  ""watermark"" text
);

CREATE INDEX IF NOT EXISTS cards_fileName ON ""cards"" (""fileName"");

CREATE INDEX IF NOT EXISTS cards_uuid ON ""cards"" (""uuid"");";
}