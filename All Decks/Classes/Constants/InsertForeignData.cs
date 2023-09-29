// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string InsertForeignData =
        @"INSERT INTO cardForeignData (
	""faceName"",
	""flavorText"",
	""language"",
	""multiverseId"",
	""name"",
	""text"",
	""type"",
	""uuid""
) VALUES (
";
}