using All_Printings.Classes;
using Combine.Classes;
using System.Text.Json;
using System.Text.Json.Serialization;

// See https://aka.ms/new-console-template for more information

const string path = "../../../../../data/AllDeckFiles";
string[] fileNames = Directory.GetFiles(path, "*.json").OrderBy(f => f).ToArray();
bool first = true;
Decks decks = new();

foreach (string fileName in fileNames)
{
    string json = File.ReadAllText(fileName);
    Deck deck = JsonSerializer.Deserialize<Deck>(json,
        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

    if (first)
    {
        decks.Meta = deck.Meta;
        first = false;
    }
    
    if (deck.Data.Commander.Count == 0)
        deck.Data.Commander = null;

    if (deck.Data.MainBoard.Count == 0)
        deck.Data.MainBoard = null;

    if (deck.Data.SideBoard.Count == 0)
        deck.Data.SideBoard = null;

    decks.Data.Add(deck.Data);
}

var options = new JsonSerializerOptions
{
    WriteIndented = false,
    AllowTrailingCommas = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(decks, options);

File.WriteAllBytes("../../../../../data/AllDecks-Lite.json", bytes);

Console.WriteLine("done");