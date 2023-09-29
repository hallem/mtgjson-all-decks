// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string InsertRulings =
        @"INSERT INTO cardRulings (
	""date"",
	""text"",
	""uuid""
) VALUES (
";
}