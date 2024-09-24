using System;
using Newtonsoft.Json;
using UnityEngine;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        // Write the Vector2 as a JSON array [x, y]
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteEndArray();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Read the JSON array and parse it into a Vector2
        float x = 0f;
        float y = 0f;

        if (reader.TokenType == JsonToken.StartArray)
        {
            reader.Read();
            x = Convert.ToSingle(reader.Value);
            reader.Read();
            y = Convert.ToSingle(reader.Value);
            reader.Read(); // Move past the end of the array
        }

        return new Vector2(x, y);
    }
}