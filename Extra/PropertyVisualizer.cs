using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class PropertyVisualizer
{
    public readonly object targetObject;
    protected VisualizerPreparation preparation;
    private Transform container;

    public PropertyVisualizer(object targetObject)
    {
        this.targetObject = targetObject;
    }

    public void Visualize(VisualizerPreparation visualizerPreparation, Transform visualizerContainer)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is null!");
            return;
        }

        preparation = visualizerPreparation;
        container = visualizerContainer;

        ClearFields();

        CreateAllFields();
    }

    private void CreateAllFields()
    {
        Type type = targetObject.GetType();

        // Field ve Property'leri MemberInfo olarak al
        MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (MemberInfo member in members)
        {
            // Check if the member has the EditableAttribute
            var editableAttribute = member.GetCustomAttribute<EditableAttribute>();
            if (editableAttribute == null) continue;
            
            switch (member)
            {
                case FieldInfo field:
                    CreateField(new VisualizerField(field, targetObject));
                    break;
                case PropertyInfo property:
                    CreateField(new VisualizerField(property, targetObject));
                    break;
            }
        }
    }

    private void CreateField(VisualizerField visualizerField)
    {
        var label = new Action(() => SetupLabel(visualizerField)); // We may not want to add label to every field.
        if (Bindings(visualizerField, label)) return;
        switch (visualizerField.value)
        {
            case Action:
                SetupButton(visualizerField);
                break;
            case Enum:
                label();
                SetupEnum(visualizerField);
                break;
            case int:
                label();
                if (visualizerField.GetAttribute<SliderMinMaxAttribute>() == null) SetupIntField(visualizerField);
                else SetupSlider(visualizerField);
                break;
            case float:
                label();
                if (visualizerField.GetAttribute<SliderMinMaxAttribute>() == null) SetupFloatField(visualizerField);
                else SetupSlider(visualizerField);
                break;
            case string:
                label();
                SetupInputField(visualizerField);
                break;
            case bool:
                label();
                SetupToggle(visualizerField);
                break;
            case Vector2:
                label();
                SetupVector2(visualizerField);
                break;
            case Color:
                label();
                SetupColor(visualizerField);
                break;
            case IListPicker:
                SetupListPicker(visualizerField);
                break;
            case IList:
                label();
                SetupList(visualizerField);
                break;
        }
    }

    private void SetupButton(VisualizerField visualizerField)
    {
        var button = InstantiateFieldUI<Button>(preparation.buttonPrefab);
        var action = (Action)visualizerField.value;
        button.GetComponentInChildren<TMP_Text>().text = StringHelpers.Spaced(visualizerField.Name);
        button.onClick.AddListener(() =>
        {
            action();
        });
    }

    protected virtual bool Bindings(VisualizerField visualizerField, Action label)
    {
        return false;
    }

    private void SetupIntField(VisualizerField visualizerField)
    {
        var inputField = InstantiateFieldUI<TMP_InputField>(preparation.inputFieldPrefab);
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        
        inputField.text = visualizerField.value.ToString();
        
        inputField.onValueChanged.AddListener(newValue =>
        {
            visualizerField.value = int.Parse(newValue);
        });
    }
    
    private void SetupFloatField(VisualizerField visualizerField)
    {
        var inputField = InstantiateFieldUI<TMP_InputField>(preparation.inputFieldPrefab);
        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
        
        inputField.text = visualizerField.value.ToString();
        
        inputField.onValueChanged.AddListener(newValue =>
        {
            visualizerField.value = float.Parse(newValue);
        });
    }

    private void SetupListPicker(VisualizerField visualizerField)
    {
        var pickerField = InstantiateFieldUI<SelectItemArea>(preparation.elementPickerButtonPrefab);
        var listPicker = (IListPicker) visualizerField.value;

        void Refresh()
        {
            pickerField.selectedItemImage.enabled = true;
            pickerField.selectedItemImage.sprite = UIHelpers.GetPrefabSpriteByComponent(listPicker.SelectedElement);
        }

        Refresh();
        pickerField.selectButton.onClick.AddListener(() =>
        {
            listPicker.onSelected = Refresh;
            MainScript.Instance.SelectItem(listPicker);
        });
    }

    private void SetupEnum(VisualizerField visualizerField)
    {
        var dropdownField = InstantiateFieldUI<TMP_Dropdown>(preparation.dropdownPrefab);
        
        var enumType = visualizerField.value.GetType();
        if (!enumType.IsEnum)
        {
            Debug.LogError("Expected an enum type.");
            return;
        }

        // Get enum names and values
        var enumNames = Enum.GetNames(enumType);
        var enumValues = Enum.GetValues(enumType);

        // Populate the dropdown with enum values
        dropdownField.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < enumNames.Length; i++)
        {
            options.Add(new TMP_Dropdown.OptionData(enumNames[i]));
        }
    
        dropdownField.AddOptions(options);

        // Set the dropdown's value based on the current enum value
        dropdownField.value = Array.IndexOf(enumValues, visualizerField.value);

        // Handle dropdown value changes
        dropdownField.onValueChanged.AddListener(selectedIndex =>
        {
            visualizerField.value = Enum.GetValues(enumType).GetValue(selectedIndex);
        });
    }

    private void SetupColor(VisualizerField visualizerField)
    {
        var fieldColor = InstantiateFieldUI<ColorSelectorFieldComponent>(preparation.colorPrefab);
        fieldColor.colorImage.color = (Color) visualizerField.value;
        fieldColor.onValueChanged = color =>
        {
            visualizerField.value = color;
        };
    }

    private void SetupVector2(VisualizerField visualizerField)
    {
        var fieldVector2 = InstantiateFieldUI<FieldVector2>(preparation.vector2Prefab);
        
        var val = (Vector2) visualizerField.value;
        
        fieldVector2.xField.text = val.x.ToString(CultureInfo.InvariantCulture);
        fieldVector2.yField.text = val.y.ToString(CultureInfo.InvariantCulture);

        fieldVector2.xField.onValueChanged.AddListener(newValue =>
        {
            val = (Vector2) visualizerField.value;
            visualizerField.value = new Vector2(float.Parse(newValue), val.y);
        });
        
        fieldVector2.yField.onValueChanged.AddListener(newValue =>
        {
            val = (Vector2) visualizerField.value;
            visualizerField.value = new Vector2(val.x, float.Parse(newValue));
        });
    }

    private void SetupLabel(VisualizerField visualizerField)
    {
        InstantiateFieldUI<TMP_Text>(preparation.labelPrefab).text = visualizerField.Name;
    }

    private void SetupList(VisualizerField visualizerField)
    {
        var listContainer = InstantiateFieldUI<ListContainer>(preparation.listContainerPrefab);
        var iList = (IList) visualizerField.value;
        listContainer.Initialize(iList, preparation);
    }

    private void SetupToggle(VisualizerField visualizerField)
    {
        var toggle = InstantiateFieldUI<Toggle>(preparation.togglePrefab);

        toggle.isOn = (bool) visualizerField.value;

        toggle.onValueChanged.AddListener(newValue =>
        {
            visualizerField.value = newValue;
        });
    }

    private void SetupInputField(VisualizerField visualizerField)
    {
        var inputField = InstantiateFieldUI<TMP_InputField>(preparation.inputFieldPrefab);

        inputField.text = visualizerField.value.ToString();
        
        inputField.onValueChanged.AddListener(newValue =>
        {
            visualizerField.value = newValue;
        });
    }

    private void SetupSlider(VisualizerField visualizerField)
    {
        var slider = InstantiateFieldUI<Slider>(preparation.sliderPrefab);
        
        var minMax = 
            visualizerField.GetAttribute<SliderMinMaxAttribute>() ?? new SliderMinMaxAttribute(0, 1);

        slider.value = (visualizerField.value switch
        {
            int i => i,
            float f => f,
            _ => 0
        }).Compress01(minMax.min, minMax.max);
        
        slider.onValueChanged.AddListener(newValue =>
        {
            var expanded = newValue.Expand(minMax.min, minMax.max);
            visualizerField.value = visualizerField.value switch
            {
                int => Mathf.FloorToInt(expanded),
                float => expanded,
                _ => visualizerField.value
            };
        });
    }

    protected T InstantiateFieldUI<T>(GameObject prefab)
    {
        return Object.Instantiate(prefab, container).GetComponentInChildren<T>();
    }

    private void ClearFields()
    {
        foreach (Transform previousElement in container)
        {
            Object.Destroy(previousElement.gameObject);
        }
    }
}

public class VisualizerField
{
    public readonly FieldInfo field;
    private readonly PropertyInfo property;
    private bool isField = true;
    private readonly object targetObject;

    public string Name => isField ? field.Name : property.Name;

    public VisualizerField(FieldInfo field, object targetObject)
    {
        this.targetObject = targetObject;
        this.field = field;
        isField = true;
    }
    
    public VisualizerField(PropertyInfo property, object targetObject)
    {
        this.targetObject = targetObject;
        this.property = property;
        isField = false;
    }

    public T GetAttribute<T>() where T : Attribute
    {
        return isField ? field.GetCustomAttribute<T>() : property.GetCustomAttribute<T>();
    }

    public object value
    {
        get
        {
            if(isField) return field.GetValue(targetObject) ?? TypeHelpers.GetDefault(field.FieldType);
            return property.GetValue(targetObject) ?? TypeHelpers.GetDefault(property.PropertyType);
        }
        set
        {
            if(isField) field.SetValue(targetObject, value);
            else property.SetValue(targetObject, value);
        }
    }
}

[Serializable]
public class VisualizerPreparation
{
    public GameObject listContainerPrefab;
    public GameObject labelPrefab;
    public GameObject inputFieldPrefab;
    public GameObject sliderPrefab;
    public GameObject togglePrefab;
    public GameObject vector2Prefab;
    public GameObject colorPrefab;
    public GameObject dropdownPrefab;
    public GameObject elementPickerButtonPrefab;
    public GameObject buttonPrefab;
}

[Serializable]
public class ListPicker<T> : IListPicker
{
    static ListPicker()
    {
        IListPicker.allListPickers.Clear();
    }

    public ListPicker()
    {
        IListPicker.allListPickers.Add(this);
    }

    public string id;

    public List<T> list;
    public void SetList(IList newList)
    {
        list = (List<T>) newList;
    }

    public int selectedIndex { get; set; }
    public object SelectedElement => list[selectedIndex];
    public Action onSelected { get; set; }
    public string listPickerID => id;
    public IList GetList => list;
}

public interface IListPicker
{
    public static List<IListPicker> allListPickers = new List<IListPicker>();
    public string listPickerID { get; }
    public IList GetList { get; }
    public object SelectedElement => GetList[selectedIndex];
    public void SetList(IList newList);
    public int selectedIndex { get; set; }
    public Action onSelected { get; set; }
}


public class EditableAttribute : RecordableAttribute
{
    
}

public class RecordableAttribute : Attribute
{
    
}