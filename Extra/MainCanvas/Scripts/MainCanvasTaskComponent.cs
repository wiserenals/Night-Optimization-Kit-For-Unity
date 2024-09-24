using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasTaskComponent : MonoBehaviour
{
    public MainCanvasTask task;
    public Slider slider;

    public TMP_Text nameText, detailsText;
    
    private void Start()
    {
        nameText.text = task.name;
    }

    private void Update()
    {
        slider.value = task.completedAmount01;
        detailsText.text = task.details;
    }
}
