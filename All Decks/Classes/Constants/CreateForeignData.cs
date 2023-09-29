// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreateForeignData = 
        @"CREATE TABLE IF NOT EXISTS ""cardForeignData"" (
  ""faceName"" text,
  ""flavorText"" text,
  ""language"" text,
  ""multiverseId"" integer,
  ""name"" text,
  ""text"" text,
  ""type"" text,
  ""uuid"" text
);

CREATE INDEX IF NOT EXISTS cardForeignData_uuid ON ""cardForeignData"" (""uuid"");";
}