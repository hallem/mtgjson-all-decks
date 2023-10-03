using All_Deck_Files.Common.Classes;
using All_Decks.Classes;

namespace Combine.Classes;

public class Decks
{
    public Meta Meta { get; set; }
    public List<Data> Data { get; set; } = new();
}
