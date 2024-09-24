using System;
using System.Linq;

public static class StringHelpers
{
    public static string RemoveStart(this string source, string value)
    {
        if (source.StartsWith(value))
        {
            return source.Substring(value.Length);
        }
        return source;
    }
    
    public static string Randomize(this string source)
    {
        Random rng = new Random();
        return new string(source.OrderBy(c => rng.Next()).ToArray());
    }

    public static string Spaced(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Create a StringBuilder to efficiently build the new string
        var spacedText = new System.Text.StringBuilder();

        // Loop through each character in the string
        for (int i = 0; i < text.Length; i++)
        {
            char currentChar = text[i];

            // If the current character is uppercase and it's not the first character, add a space before it
            if (char.IsUpper(currentChar) && i > 0)
            {
                spacedText.Append(' ');
            }

            // Append the current character
            spacedText.Append(currentChar);
        }

        return spacedText.ToString();
    }

}