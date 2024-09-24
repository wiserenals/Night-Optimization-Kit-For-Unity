using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuaternionArrayConverter : JsonConverter<Quaternion>
{
    public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.x);
        writer.WriteValue(value.y);
        writer.WriteValue(value.z);
        writer.WriteValue(value.w);
        writer.WriteEndArray();
    }

    public override Quaternion ReadJson(JsonReader reader, System.Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JArray jsonArray = JArray.Load(reader);
        float x = jsonArray[0].Value<float>();
        float y = jsonArray[1].Value<float>();
        float z = jsonArray[2].Value<float>();
        float w = jsonArray[3].Value<float>();
        return new Quaternion(x, y, z, w);
    }
}