using System.Text;

namespace All_Decks.Classes.Helpers;

// ReSharper disable ClassNeverInstantiated.Global
public class CsvList<T> : List<T>
{
    public override string ToString()
    {
        StringBuilder result = new();

        for (int i = 0; i < base.Count; i++)
        {
            result.Append(this[i]);

            if (i + 1 < base.Count)
                result.Append(", ");
        }

        return result.ToString();
    }
}