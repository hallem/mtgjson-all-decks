// ReSharper disable UseRawString
// ReSharper disable InconsistentNaming

namespace All_Decks.Classes.Constants;

public static partial class Statements
{
    public const string CreateMeta = 
        @"CREATE TABLE IF NOT EXISTS ""meta"" (
  ""date"" date,
  ""version"" text
);";
}