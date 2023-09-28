using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;
using All_Printings.Classes;
using All_Printings.Constants;

namespace All_Printings;

internal static class Program
{
    private static void Main(string[] args)
    {
        const string path = "../../../../data/AllDeckFiles";
        string[] fileNames = Directory.GetFiles(path, "*.json").OrderBy(f => f).ToArray();
        SQLiteConnection connection = new("DataSource=../../../../data/AllPrintings.sqlite;Version=3;New=False;");

        connection.Open();

        bool wasSuccessful = true;
        StringBuilder builder = new();
        
        builder.AppendLine(Statements.CreateDecksStatement);
        builder.AppendLine();
        builder.AppendLine(Statements.CreateDeckListsStatement);
        builder.AppendLine();

        for (int i = 0; i < fileNames.Length; i++)
        {
            string pathFileName = fileNames[i];
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(pathFileName);

            Console.Write("[{1}] Processing {0}: ", fileNameNoExtension, i);

            bool returnValue = ProcessDeck(builder, pathFileName, fileNameNoExtension);

            if (!returnValue)
                continue;

            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                SQLiteCommand command = connection.CreateCommand();

                try
                {
                    command.CommandText = builder.ToString();

                    int results = command.ExecuteNonQuery();

                    transaction.Commit();

                    Console.WriteLine("{0} records inserted.", results);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    wasSuccessful = false;

                    Console.WriteLine(ex.Message);
                }
            }

            if (wasSuccessful)
            {
                try
                {
                    string archivePath = Path.Combine(path, "Archive");

                    if (!Directory.Exists(archivePath))
                        Directory.CreateDirectory(archivePath);

                    string destinationFileName = Path.Combine(archivePath, Path.GetFileName(pathFileName));

                    File.Move(pathFileName, destinationFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to move the file: {0}", ex.Message);
                }
            }

            builder.Clear();
        } // end for

        connection.Close();

        Console.WriteLine("Done.");
    }

    private static bool ProcessDeck(StringBuilder builder, string pathFileName, string fileNameNoExtension)
    {
        if (File.Exists(pathFileName))
        {
            string jsonString = File.ReadAllText(pathFileName);
            Deck deck;

            try
            {
                deck = JsonSerializer.Deserialize<Deck>(jsonString,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            deck.Data.FileName = fileNameNoExtension;

            foreach (var card in deck.Data.Commander)
            {
                card.FileName = fileNameNoExtension;
                card.IsCommander = true;
            }

            foreach (var card in deck.Data.MainBoard)
            {
                card.FileName = fileNameNoExtension;
                card.IsMainBoard = true;
            }

            foreach (var card in deck.Data.SideBoard)
            {
                card.FileName = fileNameNoExtension;
                card.IsSideBoard = true;
            }

            builder.Append(Statements.InsertDecksStatement);
            builder.Append($"\t\"{deck.Data.Code}\",\n");
            builder.Append($"\t\"{deck.Data.FileName}\",\n");
            builder.Append($"\t\"{deck.Data.Name}\",\n");
            builder.Append($"\t\"{deck.Data.ReleaseDate}\",\n");
            builder.Append($"\t\"{deck.Data.Type}\"\n");
            builder.AppendLine(");");
            builder.AppendLine();

            ProcessDeck_Helper(builder, deck.Data.Commander);
            ProcessDeck_Helper(builder, deck.Data.MainBoard);
            ProcessDeck_Helper(builder, deck.Data.SideBoard);

            return true;
        }
        else
        {
            Console.WriteLine("File Not Found");

            return false;
        }
    }

    private static void ProcessDeck_Helper(StringBuilder builder, List<CardDeck> cards)
    {
        foreach (var card in cards)
        {
            builder.Append(Statements.InsertDeckListsStatement);
            builder.Append($"\t{card.Count},\n");
            builder.Append($"\t\"{card.FileName}\",\n");
            builder.Append($"\t\"{card.IsCommander}\",\n");
            builder.Append($"\t\"{card.IsFoil}\",\n");
            builder.Append($"\t\"{card.IsMainBoard}\",\n");
            builder.Append($"\t\"{card.IsSideBoard}\",\n");
            builder.Append($"\t\"{card.Uuid}\"\n");
            builder.AppendLine(");");
            builder.AppendLine();
        }
    }
}