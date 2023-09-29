// ReSharper disable InconsistentNaming
// ReSharper disable UseRawString

namespace All_Printings.Classes.Constants;

public static partial class Statements
{
    public const string CreateDeckLists =
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
}