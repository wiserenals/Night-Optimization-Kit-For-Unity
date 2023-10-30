using System.Collections.Generic;
using NOK;
using UnityEngine;

public class NightContainer : ProtectedSingleton<NightContainer>
{
    [SerializeField] [ReadOnly] private int templateIndex;
    public List<NightContainerProperties> templates;
    public static NightContainerProperties CurrentProperties => Instance._CurrentProperties;

    public void OnGUI()
    {
        templateIndex = PlayerPrefs.GetInt(NOK.Collections.TemplateIndexKey);
    }
    
    public static void SetTemplate(int index) => Instance._SetTemplate(index);

    private void _SetTemplate(int index)
    {
        if (index < 0 || index >= templates.Count)
        {
            LogWarning($"[NOK] Invalid template index ({index}). " +
                             $"Index is out of range. Please use a valid index between 0 and {templates.Count - 1}.");
            return;
        }
        templateIndex = index;
        PlayerPrefs.SetInt(NOK.Collections.TemplateIndexKey, index);
        _currentProperties = templates[index];
    }
    
    private NightContainerProperties _CurrentProperties
    {
        get
        {
            if (_currentProperties == null)
            {
                return _currentProperties = templates[PlayerPrefs.GetInt(NOK.Collections.TemplateIndexKey)];
            }

            return _currentProperties;
        }
    }
    
    private NightContainerProperties _currentProperties;
}
