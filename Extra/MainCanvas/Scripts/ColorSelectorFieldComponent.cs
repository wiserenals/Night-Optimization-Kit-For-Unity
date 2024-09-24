using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorSelectorFieldComponent : MonoBehaviour, IPointerClickHandler
{
    public Image colorImage;

    public Action<Color> onValueChanged = _ => {};


    public void OnPointerClick(PointerEventData eventData)
    {
        /*MainScript.Instance.SelectColor(x =>
        {
            colorImage.color = x;
            onValueChanged(x);
        });*/
    }
}
