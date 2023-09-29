// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreateLegalities = 
        @"CREATE TABLE IF NOT EXISTS ""cardLegalities"" (
  ""alchemy"" text,
  ""brawl"" text,
  ""commander"" text,
  ""duel"" text,
  ""explorer"" text,
  ""future"" text,
  ""gladiator"" text,
  ""historic"" text,
  ""historicbrawl"" text,
  ""legacy"" text,
  ""modern"" text,
  ""oathbreaker"" text,
  ""oldschool"" text,
  ""pauper"" text,
  ""paupercommander"" text,
  ""penny"" text,
  ""pioneer"" text,
  ""predh"" text,
  ""premodern"" text,
  ""standard"" text,
  ""uuid"" text,
  ""vintage"" text
);

CREATE INDEX IF NOT EXISTS cardLegalities_uuid ON ""cardLegalities"" (""uuid"");";
}