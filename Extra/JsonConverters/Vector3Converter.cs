using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Vector3ArrayConverter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteEndArray();
    }

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JArray jsonArray = JArray.Load(reader);
        float x = jsonArray[0].Value<float>();
        float y = jsonArray[1].Value<float>();
        float z = jsonArray[2].Value<float>();
        return new Vector3(x, y, z);
    }
}