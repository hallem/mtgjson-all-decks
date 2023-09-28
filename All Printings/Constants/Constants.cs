// ReSharper disable InconsistentNaming
// ReSharper disable UseRawString

namespace All_Printings.Constants;

public static class Statements
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
}