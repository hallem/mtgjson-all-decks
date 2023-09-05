using System.Text.Json;
using All_Decks.Classes;

// See https://aka.ms/new-console-template for more information

JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

string pathFileName = "../../../../data/AllDeckFiles/ForcesOfTheImperium_40K.json";
string fileName = Path.GetFileName(pathFileName);

if (File.Exists(pathFileName))
{
    string jsonString = File.ReadAllText(pathFileName);
    Deck deck = JsonSerializer.Deserialize<Deck>(jsonString, jsonSerializerOptions);

    deck.Data.FileName = fileName;

    foreach (var commander in deck.Data.Commander)
    {
        commander.FileName = fileName;
        commander.IsCommander = true;
    }

    foreach (var card in deck.Data.MainBoard)
    {
        card.FileName = fileName;
        card.IsMainBoard = true;
    }

    foreach (var card in deck.Data.SideBoard)
    {
        card.FileName = fileName;
        card.IsSideBoard = true;
    }

    Console.WriteLine("Done");
}
else
{
    Console.WriteLine("File Does Not Exist");
}

