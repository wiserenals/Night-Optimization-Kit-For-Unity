using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

public static class ReflectionHelpers
{
    public static List<T> GetMemberValues<T>(this object targetObject)
    {
        if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

        var memberType = typeof(T);
        var type = targetObject.GetType();
        var result = new List<T>();

        // Alanları al
        var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fieldInfos)
        {
            if (memberType.IsAssignableFrom(field.FieldType))
            {
                var fieldValue = field.GetValue(targetObject);
                if (fieldValue is T value)
                {
                    result.Add(value);
                }
            }
        }

        // Özellikleri al
        var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var prop in propertyInfos)
        {
            if (memberType.IsAssignableFrom(prop.PropertyType) && prop.CanRead)
            {
                var propertyValue = prop.GetValue(targetObject);
                if (propertyValue is T value)
                {
                    result.Add(value);
                }
            }
        }

        return result;
    }

    public static List<T> GetFieldValues<T>(this object targetObject)
    {
        if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

        var fieldType = typeof(T);
        var fieldInfos = targetObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        var result = new List<T>();

        foreach (var field in fieldInfos)
        {
            if (fieldType.IsAssignableFrom(field.FieldType))
            {
                var fieldValue = field.GetValue(targetObject);
                if (fieldValue is T value)
                {
                    result.Add(value);
                }
            }
        }

        return result;
    }
    
    public static List<T> GetPropertyValues<T>(this object targetObject)
    {
        if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

        var propertyType = typeof(T);
        var propertyInfos = targetObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        var result = new List<T>();

        foreach (var prop in propertyInfos)
        {
            if (propertyType.IsAssignableFrom(prop.PropertyType) && prop.CanRead)
            {
                var propertyValue = prop.GetValue(targetObject);
                if (propertyValue is T value)
                {
                    result.Add(value);
                }
            }
        }

        return result;
    }
    
    public static void UpdateInstanceFromDictionary(this object instance, Dictionary<string, object> data, JsonSerializerSettings settings)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        var type = instance.GetType();
        
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(prop => Attribute.IsDefined(prop, typeof(RecordableAttribute)));

        foreach (var prop in properties)
        {
            if(!prop.CanWrite) continue;
            if (data.TryGetValue(prop.Name, out var value))
            {
                var json = JsonConvert.SerializeObject(value);
                var convertedValue = JsonConvert.DeserializeObject(json, prop.PropertyType, settings);
                prop.SetValue(instance, convertedValue);
            }
        }
        
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => Attribute.IsDefined(field, typeof(RecordableAttribute)));

        foreach (var field in fields)
        {
            if (data.TryGetValue(field.Name, out var value))
            {
                var json = JsonConvert.SerializeObject(value);
                var convertedValue = JsonConvert.DeserializeObject(json, field.FieldType, settings);
                field.SetValue(instance, convertedValue);
            }
        }
    }
}