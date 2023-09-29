// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Deck_Files.Common.Classes.Constants;

public static partial class CommonStatements
{ 
    public const string CreateDecks = 
        @"CREATE TABLE IF NOT EXISTS ""decks"" (
  ""code"" text NOT NULL,
  ""fileName"" text NOT NULL,
  ""name"" text NOT NULL,
  ""releaseDate"" text,
  ""type"" text NOT NULL
);

CREATE INDEX IF NOT EXISTS decks_fileName ON ""decks"" (""fileName"");";
}