// ReSharper disable InconsistentNaming
// ReSharper disable UseRawString

namespace All_Printings.Classes.Constants;

public static partial class Statements
{
    public const string InsertDeckLists =
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