using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

public class GameConsole : Singleton<GameConsole>
{
    public TMP_InputField inputField;
    public TMP_Text debugText;

    public Transform previewParent;
    public ConsoleCommandPreview previewPrefab;

    private static List<Type> assemblyTypes;
    
    private static List<MemberInfo> assemblyMembers;

    private static List<string> assemblyTypeFullNames;

    private void Start()
    {
        assemblyTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes()
                .Where(x => x.Namespace != null && x.Namespace.StartsWith("Commands."))).ToList();

        assemblyMembers = assemblyTypes.SelectMany(x => x.GetMembers()
            .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)).ToList();

        assemblyTypeFullNames = assemblyMembers
            .ConvertAll(x => $"{x.DeclaringType.Namespace.RemoveStart("Commands.")}.{x.Name}");
        
        RenewPreviews();
        
        inputField.onValueChanged.AddListener(_ =>
        {
            RenewPreviews();
        });
    }

    private void RenewPreviews()
    {
        ClearPreviews();

        var previews = GetPossibleCommands(inputField.text);

        inputField.textComponent.color = previews.Count == 0 ? Color.red : Color.white;

        foreach (var preview in previews)
        {
            var ccp = Instantiate(previewPrefab, previewParent);
            ccp.commandPreviewText.text = preview;
        }
    }

    private void ClearPreviews()
    {
        foreach (Transform child in previewParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void InputCompleted()
    {
        var command = inputField.text;
        inputField.text = "";
        UseCommand(command);
        inputField.ActivateInputField();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) InputCompleted();
    }

    public static void Log(object message)
    {
        if (Instance.debugText != null)
        {
            Instance.debugText.text += "\n" + message;
        }
        else
        {
            Debug.Log(message); // TMP_Text atanmamışsa Unity'nin log sistemini kullan
        }
    }

    private void UseCommand(string command)
    {
        var parts = command.Trim().Split(' ');
        if (parts.Length == 0) return;

        var path = parts[0].Split('.');
        var value = parts.Length > 1 ? parts[1] : null;

        var fullNamespace = "Commands." + string.Join(".", path.Take(path.Length - 1));
        var memberName = path.Last();

        var type = assemblyTypes
            .FirstOrDefault(t => t.Namespace == fullNamespace);

        if (type == null)
        {
            Log($"Type {fullNamespace} bulunamadı.");
            return;
        }

        var member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .FirstOrDefault();

        if (member == null)
        {
            Log($"Komut {memberName} bulunamadı.");
            return;
        }

        try
        {
            if (member is PropertyInfo prop)
            {
                HandleProperty(prop, value, type);
            }
            else if (member is FieldInfo field)
            {
                HandleField(field, value, type);
            }
            else if (member is MethodInfo method)
            {
                HandleMethod(method, value, type);
            }
        }
        catch (Exception ex)
        {
            Log(ex);
        }
    }
    
    public static List<string> GetPossibleCommands(string command)
    {
        command = command.Trim().Split(' ')[0];
        if (string.IsNullOrWhiteSpace(command))
        {
            return assemblyTypeFullNames
                .ConvertAll(x => x.Split('.')[0])
                .GroupBy(x => x)
                .Select(g => g.Key + (g.Any() ? " +" + g.Count() : ""))
                .ToList();
        }
        return assemblyTypeFullNames.FindAll(x => x.StartsWith(command));
    }

    private void HandleProperty(PropertyInfo prop, string value, Type type)
    {
        var instance = type.GetProperty("Instance")?.GetValue(null);

        if (string.IsNullOrEmpty(value))
        {
            var result = prop.GetValue(instance);
            Log($"{prop.Name} = {result}");
        }
        else
        {
            var convertedValue = Convert.ChangeType(value, prop.PropertyType);
            prop.SetValue(instance, convertedValue);
            Log($"{prop.Name} = {prop.GetValue(instance)} olarak ayarlandı.");
        }
    }

    private void HandleField(FieldInfo field, string value, Type type)
    {
        var instance = type.GetProperty("Instance")?.GetValue(null);

        if (string.IsNullOrEmpty(value))
        {
            var result = field.GetValue(instance);
            Log($"{field.Name} = {result}");
        }
        else
        {
            var convertedValue = Convert.ChangeType(value, field.FieldType);
            field.SetValue(instance, convertedValue);
            Log($"{field.Name} = {field.GetValue(instance)} olarak ayarlandı.");
        }
    }

    private void HandleMethod(MethodInfo method, string value, Type type)
    {
        var instance = type.GetProperty("Instance")?.GetValue(null);

        var parameters = method.GetParameters();
        if (parameters.Length == 0)
        {
            var result = method.Invoke(instance, null);
            Log(result);
        }
        else if (parameters.Length == 1 && !string.IsNullOrEmpty(value))
        {
            var convertedValue = Convert.ChangeType(value, parameters[0].ParameterType);
            var result = method.Invoke(instance, new object[] { convertedValue });
            Log(result);
        }
        else
        {
            Log($"{method.Name} metodu için yanlış argüman sayısı.");
        }
    }
}

public class CommandAttribute : Attribute
{
}
