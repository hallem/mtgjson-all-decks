// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Deck_Files.Common.Classes.Constants;

public static partial class CommonStatements
{
	public const string InsertDecks =
		@"INSERT INTO decks (
	""code"",
	""fileName"",
	""name"",
	""releaseDate"",
	""type""
) VALUES (
";
}