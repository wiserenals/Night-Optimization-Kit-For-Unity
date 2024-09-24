using System;
using Newtonsoft.Json;
using UnityEngine;

public class ColorHexConverter : JsonConverter<Color>
{
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        // Convert the Color to a hex string and write it
        string hexColor = ColorUtility.ToHtmlStringRGBA(value);
        writer.WriteValue($"#{hexColor}");
    }

    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Read the hex string and convert it back to a Color
        string hexColor = reader.Value.ToString();
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            return color;
        }
        return existingValue; // Return existing value if parsing fails
    }
}