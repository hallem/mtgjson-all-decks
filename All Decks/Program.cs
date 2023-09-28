using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;
using All_Decks.Classes;
using All_Decks.Classes.Objects;
using All_Decks.Classes.Constants;

/*
1) Write code to work with the slim data -- DONE
2) Write code to insert the deck information into the decks table -- DONE
3) See if #2 eliminates the name encoding issues -- IT DOES
4) Find a way to centralize the json parse code into one method
5) Add create table logic controlled by a flag for the All Printings DB -- DONE
6) Add support for command line arguments
*/

namespace All_Decks;

internal static class Program
{
    private static void Main(string[] args)
    {
        const string path = "../../../../data/AllDeckFiles";
        string[] fileNames = Directory.GetFiles(path, "*.json").OrderBy(f => f).ToArray();

        SQLiteConnection connection = new ("DataSource=../../../../data/Decks.sqlite;Version=3;New=False;");

        connection.Open();

        bool wasSuccessful = true;
        StringBuilder builder = new();

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
            #region Insert Into Cards

            builder.Append(Statements.InsertCardsStatement);

            if (string.IsNullOrWhiteSpace(card.Artist))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Artist.Replace("\"", "\"\"")}\",\n");

            if (card.ArtistIds == null || card.ArtistIds.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.ArtistIds}\",\n");

            if (string.IsNullOrWhiteSpace(card.AsciiName))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.AsciiName}\",\n");

            if (string.IsNullOrWhiteSpace(card.AttractionLights))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.AttractionLights}\",\n");

            if (card.Availability == null || card.Availability.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Availability}\",\n");

            if (card.BoosterTypes == null || card.BoosterTypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.BoosterTypes}\",\n");

            if (string.IsNullOrWhiteSpace(card.BorderColor))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.BorderColor}\",\n");

            if (card.CardParts == null || card.CardParts.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.CardParts}\",\n");

            if (card.ColorIdentity == null || card.ColorIdentity.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.ColorIdentity}\",\n");

            if (card.ColorIndicator != null && card.ColorIndicator.Count > 0)
                builder.Append($"\t\"{string.Join(" ,", card.ColorIndicator)}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (card.Colors == null || card.Colors.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Colors}\",\n");

            builder.Append($"\t\"{card.Count}\",\n");

            if (card.Defense.HasValue)
                builder.Append($"\t\"{card.Defense}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (string.IsNullOrWhiteSpace(card.DuelDeck))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.DuelDeck}\",\n");

            if (card.EdhrecRank.HasValue)
                builder.Append($"\t\"{card.EdhrecRank}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (card.EdhrecSaltiness.HasValue)
                builder.Append($"\t\"{card.EdhrecSaltiness}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (card.FaceConvertedManaCost.HasValue)
                builder.Append($"\t\"{card.FaceConvertedManaCost}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (string.IsNullOrWhiteSpace(card.FaceFlavorName))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FaceFlavorName}\",\n");

            if (card.FaceManaValue.HasValue)
                builder.Append($"\t\"{card.FaceManaValue}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (string.IsNullOrWhiteSpace(card.FaceName))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FaceName}\",\n");

            if (string.IsNullOrWhiteSpace(card.FileName))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FileName}\",\n");

            if (card.Finishes == null || card.Finishes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Finishes}\",\n");

            if (string.IsNullOrWhiteSpace(card.FlavorName))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FlavorName}\",\n");

            if (string.IsNullOrWhiteSpace(card.FlavorText))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FlavorText.Replace("\"", "\"\"")}\",\n");

            if (card.FrameEffects == null || card.FrameEffects.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FrameEffects}\",\n");

            if (string.IsNullOrWhiteSpace(card.FrameVersion))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.FrameVersion}\",\n");

            if (string.IsNullOrWhiteSpace(card.Hand))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Hand}\",\n");

            if (card.HasAlternativeDeckLimit.HasValue)
                builder.Append($"\t\"{card.HasAlternativeDeckLimit}\",\n");
            else
                builder.AppendLine("\tnull,");

            if (card.HasContentWarning.HasValue)
                builder.Append($"\t\"{card.HasContentWarning}\",\n");
            else
                builder.AppendLine("\tnull,");

            builder.Append($"\t\"{card.HasFoil}\",\n");
            builder.Append($"\t\"{card.HasNonFoil}\",\n");

            if (!card.IsAlternative.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsAlternative}\",\n");

            builder.Append($"\t\"{card.IsCommander}\",\n");
            builder.Append($"\t\"{card.IsFoil}\",\n");

            if (!card.IsFullArt.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsFullArt}\",\n");

            if (!card.IsFunny.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsFunny}\",\n");

            builder.Append($"\t\"{card.IsMainBoard}\",\n");

            if (!card.IsOnlineOnly.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsOnlineOnly}\",\n");

            if (!card.IsOversized.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsOversized}\",\n");

            if (!card.IsPromo.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsPromo}\",\n");

            if (!card.IsRebalanced.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsRebalanced}\",\n");

            if (!card.IsReprint.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsReprint}\",\n");

            if (!card.IsReserved.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsReserved}\",\n");

            builder.Append($"\t\"{card.IsSideBoard}\",\n");

            if (!card.IsStarter.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsStarter}\",\n");

            if (!card.IsStorySpotlight.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsStorySpotlight}\",\n");

            if (!card.IsTextless.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsTextless}\",\n");

            if (!card.IsTimeshifted.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.IsTimeshifted}\",\n");

            if (card.Keywords == null || card.Keywords.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Keywords}\",\n");

            if (string.IsNullOrWhiteSpace(card.Language))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Language}\",\n");

            if (string.IsNullOrWhiteSpace(card.Layout))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Layout}\",\n");

            if (card.LeadershipSkills == null)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.LeadershipSkills}\",\n");

            if (string.IsNullOrWhiteSpace(card.Life))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Life}\",\n");

            if (string.IsNullOrWhiteSpace(card.Loyalty))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Loyalty}\",\n");

            if (string.IsNullOrWhiteSpace(card.ManaCost))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.ManaCost}\",\n");

            builder.Append($"\t\"{card.ManaValue}\",\n");

            if (string.IsNullOrWhiteSpace(card.Name))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Name.Replace("\"", "\"\"")}\",\n");

            if (string.IsNullOrWhiteSpace(card.Number))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Number}\",\n");

            if (card.OriginalPrintings == null || card.OriginalPrintings.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.OriginalPrintings}\",\n");

            if (string.IsNullOrWhiteSpace(card.OriginalReleaseDate))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.OriginalReleaseDate}\",\n");

            if (string.IsNullOrEmpty(card.OriginalText))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.OriginalText.Replace("\"", "\"\"")}\",\n");

            if (string.IsNullOrWhiteSpace(card.OriginalType))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.OriginalType}\",\n");

            if (card.OtherFaceIds == null || card.OtherFaceIds.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.OtherFaceIds}\",\n");

            if (string.IsNullOrWhiteSpace(card.Power))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Power}\",\n");

            if (card.Printings == null || card.Printings.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Printings}\",\n");

            if (card.PromoTypes == null || card.PromoTypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.PromoTypes}\",\n");

            if (string.IsNullOrWhiteSpace(card.Rarity))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Rarity}\",\n");

            if (card.RebalancedPrintings == null || card.RebalancedPrintings.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.RebalancedPrintings}\",\n");

            if (card.RelatedCards == null)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.RelatedCards}\",\n");

            if (string.IsNullOrWhiteSpace(card.SecurityStamp))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.SecurityStamp}\",\n");

            if (string.IsNullOrWhiteSpace(card.SetCode))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.SetCode}\",\n");

            if (string.IsNullOrWhiteSpace(card.Side))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Side}\",\n");

            if (string.IsNullOrWhiteSpace(card.Signature))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Signature.Replace("\"", "\"\"")}\",\n");

            if (card.SourceProducts == null)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.SourceProducts}\",\n");

            if (card.Subsets == null || card.Subsets.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Subsets}\",\n");

            if (card.Subtypes == null || card.Subtypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Subtypes}\",\n");

            if (card.Supertypes == null || card.Supertypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Supertypes}\",\n");

            if (string.IsNullOrWhiteSpace(card.Text))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Text.Replace("\"", "\"\"")}\",\n");

            if (string.IsNullOrWhiteSpace(card.Toughness))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Toughness}\",\n");

            if (string.IsNullOrWhiteSpace(card.Type))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Type}\",\n");

            if (card.Types == null || card.Types.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Types}\",\n");

            if (string.IsNullOrWhiteSpace(card.Uuid))
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Uuid}\",\n");

            if (card.Variations == null || card.Variations.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.Append($"\t\"{card.Variations}\",\n");

            if (string.IsNullOrWhiteSpace(card.Watermark))
                builder.AppendLine("\tnull");
            else
                builder.Append($"\t\"{card.Watermark}\"\n");

            builder.AppendLine(");");
            builder.AppendLine();

            #endregion

            #region Foreign Data

            if (card.ForeignData?.Count > 0)
            {
                foreach (var foreignData in card.ForeignData)
                {
                    builder.Append(Statements.InsertForeignDataStatement);

                    if (string.IsNullOrWhiteSpace(foreignData.FaceName))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{foreignData.FaceName}\",\n");

                    if (string.IsNullOrWhiteSpace(foreignData.FlavorText))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{foreignData.FlavorText.Replace("\"", "\"\"")}\",\n");

                    if (string.IsNullOrWhiteSpace(foreignData.Language))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{foreignData.Language}\",\n");

                    builder.Append($"\t{foreignData.MultiverseId},\n");

                    if (string.IsNullOrWhiteSpace(foreignData.Name))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{foreignData.Name.Replace("\"", "\"\"")}\",\n");

                    if (string.IsNullOrWhiteSpace(foreignData.Text))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{foreignData.Text.Replace("\"", "\"\"")}\",\n");

                    if (string.IsNullOrWhiteSpace(foreignData.Type))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{foreignData.Type}\",\n");

                    builder.Append($"\t\"{card.Uuid}\"\n");

                    builder.AppendLine(");");
                }

                builder.AppendLine();
            }

            #endregion

            #region Identifiers

            if (card.Identifiers != null)
            {
                builder.Append(Statements.InsertIdentifiersStatement);

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardKingdomEtchedId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.CardKingdomEtchedId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardKingdomFoilId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.CardKingdomFoilId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardKingdomId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.CardKingdomId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardsphereId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.CardsphereId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.McmId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.McmId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.McmMetaId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.McmMetaId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgArenaId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MtgArenaId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgjsonFoilVersionId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MtgjsonFoilVersionId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgjsonNonFoilVersionId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MtgjsonNonFoilVersionId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgjsonV4Id))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MtgjsonV4Id}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgoFoilId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MtgoFoilId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgoId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MtgoId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.MultiverseId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.MultiverseId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.ScryfallId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.ScryfallId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.ScryfallIllustrationId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.ScryfallIllustrationId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.ScryfallOracleId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.ScryfallOracleId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.TcgplayerEtchedProductId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.TcgplayerEtchedProductId}\",\n");

                if (string.IsNullOrWhiteSpace(card.Identifiers.TcgplayerProductId))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Identifiers.TcgplayerProductId}\",\n");

                builder.Append($"\t\"{card.Uuid}\"\n");

                builder.AppendLine(");");
                builder.AppendLine();
            }

            #endregion

            #region Legalities

            if (card.Legalities != null)
            {
                builder.Append(Statements.InsertLegalitiesStatement);

                if (string.IsNullOrWhiteSpace(card.Legalities.Alchemy))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Alchemy}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Brawl))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Brawl}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Commander))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Commander}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Duel))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Duel}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Explorer))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Explorer}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Future))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Future}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Gladiator))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Gladiator}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Historic))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Historic}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.HistoricBrawl))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.HistoricBrawl}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Legacy))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Legacy}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Modern))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Modern}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.OathBreaker))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.OathBreaker}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.OldSchool))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.OldSchool}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Pauper))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Pauper}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.PauperCommander))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.PauperCommander}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Penny))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Penny}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Pioneer))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Pioneer}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Predh))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Predh}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.PreModern))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.PreModern}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Standard))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.Legalities.Standard}\",\n");

                builder.Append($"\t\"{card.Uuid}\",\n");

                if (string.IsNullOrWhiteSpace(card.Legalities.Vintage))
                    builder.AppendLine("\tnull");
                else
                    builder.Append($"\t\"{card.Legalities.Vintage}\"\n");

                builder.AppendLine(");");
                builder.AppendLine();
            }

            #endregion

            #region Purchase Urls

            if (card.PurchaseUrls != null)
            {
                builder.Append(Statements.InsertPurchaseUrlsStatement);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardKingdom))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.PurchaseUrls.CardKingdom}\",\n");

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardKingdomEtched))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.PurchaseUrls.CardKingdomEtched}\",\n");

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardKingdomFoil))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.PurchaseUrls.CardKingdomFoil}\",\n");

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardMarket))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.PurchaseUrls.CardMarket}\",\n");

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.Tcgplayer))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.PurchaseUrls.Tcgplayer}\",\n");

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.TcgplayerEtched))
                    builder.AppendLine("\tnull,");
                else
                    builder.Append($"\t\"{card.PurchaseUrls.TcgplayerEtched}\",\n");

                builder.Append($"\t\"{card.Uuid}\"\n");

                builder.AppendLine(");");
                builder.AppendLine();
            }

            #endregion

            #region Rulings

            if (card.Rulings?.Count > 0)
            {
                foreach (var ruling in card.Rulings)
                {
                    builder.Append(Statements.InsertRulingsStatement);

                    if (string.IsNullOrWhiteSpace(ruling.Date))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{ruling.Date}\",\n");

                    if (string.IsNullOrWhiteSpace(ruling.Text))
                        builder.AppendLine("\tnull,");
                    else
                        builder.Append($"\t\"{ruling.Text.Replace("\"", "\"\"")}\",\n");

                    builder.Append($"\t\"{card.Uuid}\"\n");

                    builder.AppendLine(");");
                    builder.AppendLine();
                }
            }

            #endregion
        }
    }
}