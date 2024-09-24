using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementPickerPanel : MonoBehaviour
{
    public Transform content;
    public PickerItem pickerItemPrefab;

    public Action onValueChanged;
    private Action onSelected;
    private int selectedItemIndex = 0;

    public void Show(object obj)
    {
        var picker = (IListPicker) obj;
        var elements = picker.GetList;
        
        gameObject.SetActive(true);
        
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        
        var i = 0;
        foreach (var element in elements)
        {
            var pickerItem = Instantiate(pickerItemPrefab, content);
            pickerItem.Initialize(this);
            pickerItem.index = i;
            pickerItem.SetItem(i.ToString(), element);
            i++;
        }

        onSelected = () =>
        {
            picker.selectedIndex = selectedItemIndex;
            picker.onSelected();
        };
    }

    public void Apply()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        onValueChanged = () => { };

        onSelected = () => { };

        selectedItemIndex = 0;
        
        gameObject.SetActive(false);
    }

    public void Select(PickerItem pickerItem)
    {
        foreach (Transform child in content)
        {
            if (child.TryGetComponent<PickerItem>(out var item)) item.Deselect();
        }

        pickerItem.EnableSelectionOutline();

        selectedItemIndex = pickerItem.index;
        
        onSelected();
    }
}
