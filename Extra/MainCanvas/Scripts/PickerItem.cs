using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PickerItem : MonoBehaviour, IPointerClickHandler
{
    public int index = 0;
    public Image selectionImage;
    public Image itemImage;
    public TMP_Text itemNameText;
    private object targetItem;
    private ElementPickerPanel elementPickerPanel;

    public void Initialize(ElementPickerPanel pickerPanel)
    {
        elementPickerPanel = pickerPanel;
    }

    public void SetItem(string itemName, object item)
    {
        targetItem = item;

        if (item is Sprite sprite) itemImage.sprite = sprite;
        else
        {
            itemImage.sprite = UIHelpers.GetPrefabSpriteByComponent(item);
        }

        itemNameText.text = itemName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        elementPickerPanel.Select(this);
    }

    public void Deselect()
    {
        selectionImage.enabled = false;
    }

    public void EnableSelectionOutline()
    {
        selectionImage.enabled = true;
    }
}
