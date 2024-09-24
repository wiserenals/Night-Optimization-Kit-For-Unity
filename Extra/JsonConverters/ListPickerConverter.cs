using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class ListPickerConverter : JsonConverter<IListPicker>
{
    public override void WriteJson(JsonWriter writer, IListPicker value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.listPickerID);
        writer.WriteValue(value.selectedIndex);
        writer.WriteEndArray();
    }

    public override IListPicker ReadJson(JsonReader reader, Type objectType, IListPicker existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Read the JSON array and parse it into a Vector2
        string pickerID = "";
        int selectedIndex = 0;

        if (reader.TokenType == JsonToken.StartArray)
        {
            reader.Read();
            pickerID = reader.Value.ToString();
            reader.Read();
            selectedIndex = Convert.ToInt32(reader.Value);
            reader.Read();
        }

        var listPicker = IListPicker.allListPickers.Find(x => x.listPickerID == pickerID).CloneListPicker();
        listPicker.selectedIndex = selectedIndex;
        
        return listPicker;
    }
}