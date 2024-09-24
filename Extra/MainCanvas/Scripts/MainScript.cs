using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : SingletonDontDestroy<MainScript>
{
    public GraphicRaycaster mainRaycaster;
    /*public UIOutline outlinePrefab;*/
    /*public ColorSelector colorSelector;*/
    public ElementPickerPanel elementPicker;
    public GameConsole gameConsole;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Tab))
        {
            gameConsole.gameObject.SetActive(!gameConsole.gameObject.activeSelf);
            gameConsole.inputField.text = "";
        }
    }

    public void SelectItem(object list, Action onEndSelect = null)
    {
        elementPicker.Show(list);
    }
    
    /*
    public void SelectColor(Action<Color> onColorChanged, Action<Color> onEndEdit = null)
    {
        if (colorSelector.gameObject.activeSelf) return;
        
        colorSelector.gameObject.SetActive(true);
        
        colorSelector.colorPicker.onColorChange.AddListener(color =>
        {
            try
            {
                onColorChanged(color);
            }
            catch (Exception)
            {
                colorSelector.colorPicker.Close();
            }
            
        });
    }
    */



}
