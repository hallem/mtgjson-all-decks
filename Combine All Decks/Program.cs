using All_Decks.Classes;
using Combine.Classes;
using System.Text.Json;
using System.Text.Json.Serialization;
using All_Decks.Classes.Objects;

// See https://aka.ms/new-console-template for more information

const string path = "../../../../../data/AllDeckFiles";
string[] fileNames = Directory.GetFiles(path, "*.json").OrderBy(f => f).ToArray();
bool first = true;
Decks decks = new();

foreach (string fileName in fileNames)
{
    string json = File.ReadAllText(fileName);
    Deck deck = JsonSerializer.Deserialize<Deck>(json,
        new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

    if (first)
    {
        decks.Meta = deck.Meta;
        first = false;
    }

    if (deck.Data.Commander.Count == 0)
        deck.Data.Commander = null;
    else
        foreach (var card in deck.Data.Commander)
            SerializeHelper(card);

    if (deck.Data.MainBoard.Count == 0)
        deck.Data.MainBoard = null;
    else
        foreach (var card in deck.Data.MainBoard)
            SerializeHelper(card);

    if (deck.Data.SideBoard.Count == 0)
        deck.Data.SideBoard = null;
    else
        foreach (var card in deck.Data.SideBoard)
            SerializeHelper(card);

    decks.Data.Add(deck.Data);
}

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    AllowTrailingCommas = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(decks, options);

File.WriteAllBytes("../../../../../data/AllDecks.json", bytes);

Console.WriteLine("done");
return;

static void SerializeHelper(CardDeck card)
{
    if (card.ArtistIds?.Count == 0)
        card.ArtistIds = null;

    if (card.Availability?.Count == 0)
        card.Availability = null;

    if (card.BoosterTypes?.Count == 0)
        card.BoosterTypes = null;

    if (card.CardParts?.Count == 0)
        card.CardParts = null;

    if (card.ColorIdentity?.Count == 0)
        card.ColorIdentity = null;

    if (card.ColorIndicator?.Count == 0)
        card.ColorIndicator = null;

    if (card.Colors?.Count == 0)
        card.Colors = null;

    if (card.Finishes?.Count == 0)
        card.Finishes = null;

    if (card.ForeignData?.Count == 0)
        card.ForeignData = null;

    if (card.FrameEffects?.Count == 0)
        card.FrameEffects = null;

    if (card.Keywords?.Count == 0)
        card.Keywords = null;

    if (card.OriginalPrintings?.Count == 0)
        card.OriginalPrintings = null;

    if (card.OtherFaceIds?.Count == 0)
        card.OtherFaceIds = null;

    if (card.Printings?.Count == 0)
        card.Printings = null;

    if (card.PromoTypes?.Count == 0)
        card.PromoTypes = null;

    if (card.RebalancedPrintings?.Count == 0)
        card.RebalancedPrintings = null;

    if (card.Rulings?.Count == 0)
        card.Rulings = null;

    if (card.Subsets?.Count == 0)
        card.Subsets = null;

    if (card.Subtypes?.Count == 0)
        card.Subtypes = null;

    if (card.Supertypes?.Count == 0)
        card.Supertypes = null;

    if (card.Types?.Count == 0)
        card.Types = null;

    if (card.Variations?.Count == 0)
        card.Variations = null;
}
