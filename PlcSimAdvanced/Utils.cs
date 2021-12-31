using System.Text;

namespace PlcSimAdvanced;

public static class Utils
{
    /// <summary>
    /// Convert a array with data block names into a string that is suitable for the Siemens API
    /// </summary>
    /// <param name="dataBlockNames">Array from data block names</param>
    /// <returns>The string that contains the data block in a suitable format</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetDataBlockFilter(string[] dataBlockNames)
    {
        // TODO: Check if this works like intended
        if (dataBlockNames.Length == 0)
            throw new ArgumentException(ExceptionUtils.EmptyCollectionMessage, nameof(dataBlockNames));

        StringBuilder stringBuilder = new();

        foreach (var dataBlockName in dataBlockNames)
        {
            stringBuilder.Append($"\"{dataBlockName}\",");
        }

        return stringBuilder.ToString();
    }

    public static string GetIp(string[]? array)
    {
        if (array?.Length is not 4)
            return "0.0.0.0";

        StringBuilder builder = new();
        
        foreach (var s in array)
        {
            builder.Append(s);
            builder.Append('.');
        }

        return builder.ToString();
    }
}