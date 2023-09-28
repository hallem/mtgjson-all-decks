// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace All_Printings.Classes;

public class CardDeck
{
    public int Count { get; set; }
    public string FileName { get; set; }
    public bool IsCommander { get; set; }
    public bool IsFoil { get; set; }
    public bool IsMainBoard { get; set; }
    public bool IsSideBoard { get; set; }
    public string Uuid { get; set; }
}
