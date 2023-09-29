// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreateRulings = 
        @"CREATE TABLE IF NOT EXISTS ""cardRulings"" (
  ""date"" date,
  ""text"" text,
  ""uuid"" varchar(36) NOT NULL
);

CREATE INDEX IF NOT EXISTS cardRulings_uuid ON ""cardRulings"" (""uuid"");";
}