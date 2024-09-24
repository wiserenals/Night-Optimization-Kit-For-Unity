using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class JsonSerializeRecordableAttributeResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        // Get all properties for the type
        var properties = base.CreateProperties(type, memberSerialization);

        // Only serialize properties with the JsonSerializeOnlyAttribute
        return properties.Where(p => 
            p.AttributeProvider != null && p.AttributeProvider.GetAttributes(typeof(RecordableAttribute), true).Any()).ToList();
    }
}