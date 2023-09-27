using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;
using AllDecks.Classes;
using AllDecks.Constants;
using AllDecks.Objects;

/*
1) Write code to work with the slim data -- DONE
2) Write code to insert the deck informaiton into the decks table -- DONE
3) See if #2 eliminates the name encoding issues -- IT DOES
4) Find a way to centralize the json parse code into one method
5) Add create table logic controlled by a flag for the All Printings DB
*/

internal class Program
{
    private static void Main(string[] args)
    {
        bool processSlim = false;
        bool archiveFiles = true;
        string path = "../../../../data/AllDeckFiles";
        string[] fileNames = Directory.GetFiles(path, "*.json").OrderBy(f => f).ToArray();

        SQLiteConnection connection = new("DataSource=../../../../data/Decks.sqlite;Version=3;New=False;");
        //SQLiteConnection connection = new("DataSource=../../../../data/AllPrintings.sqlite;Version=3;New=False;");

        connection.Open();

        bool wasSuccessfull = true,
            returnValue;
        StringBuilder builder = new();

        for (int i = 0; i < fileNames.Length; i++)
        {
            string pathFileName = fileNames[i];
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(pathFileName);

            Console.Write("[{1}] Processing {0}: ", fileNameNoExtension, i);

            if (processSlim)
                returnValue = ProcessDeckSlim(builder, pathFileName, fileNameNoExtension);
            else
                returnValue = ProcessDeck(builder, pathFileName, fileNameNoExtension);

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

                    wasSuccessfull = false;

                    Console.WriteLine(ex.Message);
                }
            }

            if (wasSuccessfull && archiveFiles)
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

    private static void Process_Helper(StringBuilder builder, DataCommon data)
    {
        builder.Append(Constants.insertDecksStatement);
        builder.AppendFormat("\t\"{0}\",\n", data.Code);
        builder.AppendFormat("\t\"{0}\",\n", data.FileName);
        builder.AppendFormat("\t\"{0}\",\n", data.Name);
        builder.AppendFormat("\t\"{0}\",\n", data.ReleaseDate);
        builder.AppendFormat("\t\"{0}\"\n", data.Type);
        builder.AppendLine(");");
        builder.AppendLine();
    }

    private static void ProcessDeckSlim_Helper(StringBuilder builder, List<CardDeckSlim> cards)
    {
        foreach (var card in cards)
        {
            builder.Append(Constants.insertDeckListsStatement);
            builder.AppendFormat("\t{0},\n", card.Count);
            builder.AppendFormat("\t\"{0}\",\n", card.FileName);
            builder.AppendFormat("\t\"{0}\",\n", card.IsCommander);
            builder.AppendFormat("\t\"{0}\",\n", card.IsFoil);
            builder.AppendFormat("\t\"{0}\",\n", card.IsMainBoard);
            builder.AppendFormat("\t\"{0}\",\n", card.IsSideBoard);
            builder.AppendFormat("\t\"{0}\"\n", card.UUID);
            builder.AppendLine(");");
            builder.AppendLine();
        }
    }

    private static bool ProcessDeckSlim(StringBuilder builder, string pathFileName, string fileNameNoExtension)
    {
        if (File.Exists(pathFileName))
        {
            string jsonString = File.ReadAllText(pathFileName);
            DeckSlim deck;

            try
            {
                deck = JsonSerializer.Deserialize<DeckSlim>(jsonString,
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

            Process_Helper(builder, deck.Data);
            ProcessDeckSlim_Helper(builder, deck.Data.Commander);
            ProcessDeckSlim_Helper(builder, deck.Data.MainBoard);
            ProcessDeckSlim_Helper(builder, deck.Data.SideBoard);

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

            builder.Append(Constants.insertCardsStatement);

            if (string.IsNullOrWhiteSpace(card.Artist))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Artist.Replace("\"", "\"\""));

            if (card.ArtistIds == null || card.ArtistIds.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.ArtistIds);

            if (string.IsNullOrWhiteSpace(card.AsciiName))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.AsciiName);

            if (string.IsNullOrWhiteSpace(card.AttractionLights))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.AttractionLights);

            if (card.Availability == null || card.Availability.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Availability);

            if (card.BoosterTypes == null || card.BoosterTypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.BoosterTypes);

            if (string.IsNullOrWhiteSpace(card.BorderColor))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.BorderColor);

            if (card.CardParts == null || card.CardParts.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.CardParts);

            if (card.ColorIdentity == null || card.ColorIdentity.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.ColorIdentity);

            if (card.ColorIndicator != null && card.ColorIndicator.Count > 0)
                builder.AppendFormat("\t\"{0}\",\n", string.Join(" ,", card.ColorIndicator));
            else
                builder.AppendLine("\tnull,");

            if (card.Colors == null || card.Colors.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Colors);

            builder.AppendFormat("\t\"{0}\",\n", card.Count);

            if (card.Defense.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.Defense);
            else
                builder.AppendLine("\tnull,");

            if (string.IsNullOrWhiteSpace(card.DuelDeck))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.DuelDeck);

            if (card.EdhrecRank.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.EdhrecRank);
            else
                builder.AppendLine("\tnull,");

            if (card.EdhrecSaltiness.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.EdhrecSaltiness);
            else
                builder.AppendLine("\tnull,");

            if (card.FaceConvertedManaCost.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.FaceConvertedManaCost);
            else
                builder.AppendLine("\tnull,");

            if (string.IsNullOrWhiteSpace(card.FaceFlavorName))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FaceFlavorName);

            if (card.FaceManaValue.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.FaceManaValue);
            else
                builder.AppendLine("\tnull,");

            if (string.IsNullOrWhiteSpace(card.FaceName))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FaceName);

            if (string.IsNullOrWhiteSpace(card.FileName))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FileName);

            if (card.Finishes == null || card.Finishes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Finishes);

            if (string.IsNullOrWhiteSpace(card.FlavorName))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FlavorName);

            if (string.IsNullOrWhiteSpace(card.FlavorText))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FlavorText.Replace("\"", "\"\""));

            if (card.FrameEffects == null || card.FrameEffects.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FrameEffects);

            if (string.IsNullOrWhiteSpace(card.FrameVersion))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.FrameVersion);

            if (string.IsNullOrWhiteSpace(card.Hand))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Hand);

            if (card.HasAlternativeDeckLimit.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.HasAlternativeDeckLimit);
            else
                builder.AppendLine("\tnull,");

            if (card.HasContentWarning.HasValue)
                builder.AppendFormat("\t\"{0}\",\n", card.HasContentWarning);
            else
                builder.AppendLine("\tnull,");

            builder.AppendFormat("\t\"{0}\",\n", card.HasFoil);
            builder.AppendFormat("\t\"{0}\",\n", card.HasNonFoil);

            if (!card.IsAlternative.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsAlternative);

            builder.AppendFormat("\t\"{0}\",\n", card.IsCommander);
            builder.AppendFormat("\t\"{0}\",\n", card.IsFoil);

            if (!card.IsFullArt.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsFullArt);

            if (!card.IsFunny.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsFunny);

            builder.AppendFormat("\t\"{0}\",\n", card.IsMainBoard);

            if (!card.IsOnlineOnly.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsOnlineOnly);

            if (!card.IsOversized.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsOversized);

            if (!card.IsPromo.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsPromo);

            if (!card.IsRebalanced.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsRebalanced);

            if (!card.IsReprint.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsReprint);

            if (!card.IsReserved.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsReserved);

            builder.AppendFormat("\t\"{0}\",\n", card.IsSideBoard);

            if (!card.IsStarter.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsStarter);

            if (!card.IsStorySpotlight.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsStorySpotlight);

            if (!card.IsTextless.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsTextless);

            if (!card.IsTimeshifted.HasValue)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.IsTimeshifted);

            if (card.Keywords == null || card.Keywords.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Keywords);

            if (string.IsNullOrWhiteSpace(card.Language))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Language);

            if (string.IsNullOrWhiteSpace(card.Layout))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Layout);

            if (card.LeadershipSkills == null)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.LeadershipSkills);

            if (string.IsNullOrWhiteSpace(card.Life))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Life);

            if (string.IsNullOrWhiteSpace(card.Loyalty))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Loyalty);

            if (string.IsNullOrWhiteSpace(card.ManaCost))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.ManaCost);

            builder.AppendFormat("\t\"{0}\",\n", card.ManaValue);

            if (string.IsNullOrWhiteSpace(card.Name))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Name.Replace("\"", "\"\""));

            if (string.IsNullOrWhiteSpace(card.Number))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Number);

            if (card.OriginalPrintings == null || card.OriginalPrintings.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.OriginalPrintings);

            if (string.IsNullOrWhiteSpace(card.OriginalReleaseDate))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.OriginalReleaseDate);

            if (string.IsNullOrEmpty(card.OriginalText))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.OriginalText.Replace("\"", "\"\""));

            if (string.IsNullOrWhiteSpace(card.OriginalType))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.OriginalType);

            if (card.OtherFaceIds == null || card.OtherFaceIds.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.OtherFaceIds);

            if (string.IsNullOrWhiteSpace(card.Power))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Power);

            if (card.Printings == null || card.Printings.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Printings);

            if (card.PromoTypes == null || card.PromoTypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.PromoTypes);

            if (string.IsNullOrWhiteSpace(card.Rarity))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Rarity);

            if (card.RebalancedPrintings == null || card.RebalancedPrintings.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.RebalancedPrintings);

            if (card.RelatedCards == null)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.RelatedCards);

            if (string.IsNullOrWhiteSpace(card.SecurityStamp))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.SecurityStamp);

            if (string.IsNullOrWhiteSpace(card.SetCode))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.SetCode);

            if (string.IsNullOrWhiteSpace(card.Side))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Side);

            if (string.IsNullOrWhiteSpace(card.Signature))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Signature.Replace("\"", "\"\""));

            if (card.SourceProducts == null)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.SourceProducts);

            if (card.Subsets == null || card.Subsets.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Subsets);

            if (card.Subtypes == null || card.Subtypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Subtypes);

            if (card.Supertypes == null || card.Supertypes.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Supertypes);

            if (string.IsNullOrWhiteSpace(card.Text))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Text.Replace("\"", "\"\""));

            if (string.IsNullOrWhiteSpace(card.Toughness))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Toughness);

            if (string.IsNullOrWhiteSpace(card.Type))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Type);

            if (card.Types == null || card.Types.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Types);

            if (string.IsNullOrWhiteSpace(card.UUID))
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.UUID);

            if (card.Variations == null || card.Variations.Count == 0)
                builder.AppendLine("\tnull,");
            else
                builder.AppendFormat("\t\"{0}\",\n", card.Variations);

            if (string.IsNullOrWhiteSpace(card.Watermark))
                builder.AppendLine("\tnull");
            else
                builder.AppendFormat("\t\"{0}\"\n", card.Watermark);

            builder.AppendLine(");");
            builder.AppendLine();

            #endregion

            #region Foreign Data

            if (card.ForeignData?.Count > 0)
            {
                foreach (var foreignData in card.ForeignData)
                {
                    builder.Append(Constants.insertForeignDataStatement);

                    if (string.IsNullOrWhiteSpace(foreignData.FaceName))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", foreignData.FaceName);

                    if (string.IsNullOrWhiteSpace(foreignData.FlavorText))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", foreignData.FlavorText.Replace("\"", "\"\""));

                    if (string.IsNullOrWhiteSpace(foreignData.Language))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", foreignData.Language);

                    builder.AppendFormat("\t{0},\n", foreignData.MultiverseId);

                    if (string.IsNullOrWhiteSpace(foreignData.Name))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", foreignData.Name.Replace("\"", "\"\""));

                    if (string.IsNullOrWhiteSpace(foreignData.Text))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", foreignData.Text.Replace("\"", "\"\""));

                    if (string.IsNullOrWhiteSpace(foreignData.Type))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", foreignData.Type);

                    builder.AppendFormat("\t\"{0}\"\n", card.UUID);

                    builder.AppendLine(");");
                }

                builder.AppendLine();
            }

            #endregion

            #region Identifiers

            if (card.Identifiers != null)
            {
                builder.Append(Constants.insertIdentifiersStatement);

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardKingdomEtchedId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.CardKingdomEtchedId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardKingdomFoilId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.CardKingdomFoilId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardKingdomId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.CardKingdomId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.CardsphereId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.CardsphereId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.McmId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.McmId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.McmMetaId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.McmMetaId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgArenaId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MtgArenaId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgjsonFoilVersionId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MtgjsonFoilVersionId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgjsonNonFoilVersionId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MtgjsonNonFoilVersionId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgjsonV4Id))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MtgjsonV4Id);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgoFoilId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MtgoFoilId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MtgoId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MtgoId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.MultiverseId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.MultiverseId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.ScryfallId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.ScryfallId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.ScryfallIllustrationId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.ScryfallIllustrationId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.ScryfallOracleId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.ScryfallOracleId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.TcgplayerEtchedProductId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.TcgplayerEtchedProductId);

                if (string.IsNullOrWhiteSpace(card.Identifiers.TcgplayerProductId))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Identifiers.TcgplayerProductId);

                builder.AppendFormat("\t\"{0}\"\n", card.UUID);

                builder.AppendLine(");");
                builder.AppendLine();
            }

            #endregion

            #region Legalities

            if (card.Legalities != null)
            {
                builder.Append(Constants.insertLegalitiesStatement);

                if (string.IsNullOrWhiteSpace(card.Legalities.Alchemy))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Alchemy);

                if (string.IsNullOrWhiteSpace(card.Legalities.Brawl))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Brawl);

                if (string.IsNullOrWhiteSpace(card.Legalities.Commander))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Commander);

                if (string.IsNullOrWhiteSpace(card.Legalities.Duel))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Duel);

                if (string.IsNullOrWhiteSpace(card.Legalities.Explorer))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Explorer);

                if (string.IsNullOrWhiteSpace(card.Legalities.Future))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Future);

                if (string.IsNullOrWhiteSpace(card.Legalities.Gladiator))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Gladiator);

                if (string.IsNullOrWhiteSpace(card.Legalities.Historic))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Historic);

                if (string.IsNullOrWhiteSpace(card.Legalities.HistoricBrawl))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.HistoricBrawl);

                if (string.IsNullOrWhiteSpace(card.Legalities.Legacy))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Legacy);

                if (string.IsNullOrWhiteSpace(card.Legalities.Modern))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Modern);

                if (string.IsNullOrWhiteSpace(card.Legalities.OathBreaker))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.OathBreaker);

                if (string.IsNullOrWhiteSpace(card.Legalities.OldSchool))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.OldSchool);

                if (string.IsNullOrWhiteSpace(card.Legalities.Pauper))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Pauper);

                if (string.IsNullOrWhiteSpace(card.Legalities.PauperCommander))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.PauperCommander);

                if (string.IsNullOrWhiteSpace(card.Legalities.Penny))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Penny);

                if (string.IsNullOrWhiteSpace(card.Legalities.Pioneer))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Pioneer);

                if (string.IsNullOrWhiteSpace(card.Legalities.Predh))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Predh);

                if (string.IsNullOrWhiteSpace(card.Legalities.PreModern))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.PreModern);

                if (string.IsNullOrWhiteSpace(card.Legalities.Standard))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.Legalities.Standard);

                builder.AppendFormat("\t\"{0}\",\n", card.UUID);

                if (string.IsNullOrWhiteSpace(card.Legalities.Vintage))
                    builder.AppendLine("\tnull");
                else
                    builder.AppendFormat("\t\"{0}\"\n", card.Legalities.Vintage);

                builder.AppendLine(");");
                builder.AppendLine();
            }

            #endregion

            #region Purchase Urls

            if (card.PurchaseUrls != null)
            {
                builder.Append(Constants.insertPurchaseUrlsStatement);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardKingdom))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.PurchaseUrls.CardKingdom);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardKingdomEtched))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.PurchaseUrls.CardKingdomEtched);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardKingdomFoil))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.PurchaseUrls.CardKingdomFoil);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.CardMarket))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.PurchaseUrls.CardMarket);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.Tcgplayer))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.PurchaseUrls.Tcgplayer);

                if (string.IsNullOrWhiteSpace(card.PurchaseUrls.TcgplayerEtched))
                    builder.AppendLine("\tnull,");
                else
                    builder.AppendFormat("\t\"{0}\",\n", card.PurchaseUrls.TcgplayerEtched);

                builder.AppendFormat("\t\"{0}\"\n", card.UUID);

                builder.AppendLine(");");
                builder.AppendLine();
            }

            #endregion

            #region Rulings

            if (card.Rulings?.Count > 0)
            {
                foreach (var ruling in card.Rulings)
                {
                    builder.Append(Constants.insertRulingsStatement);

                    if (string.IsNullOrWhiteSpace(ruling.Date))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", ruling.Date);

                    if (string.IsNullOrWhiteSpace(ruling.Text))
                        builder.AppendLine("\tnull,");
                    else
                        builder.AppendFormat("\t\"{0}\",\n", ruling.Text.Replace("\"", "\"\""));

                    builder.AppendFormat("\t\"{0}\"\n", card.UUID);

                    builder.AppendLine(");");
                    builder.AppendLine();
                }
            }

            #endregion
        }
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

            Process_Helper(builder, deck.Data);
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
}
