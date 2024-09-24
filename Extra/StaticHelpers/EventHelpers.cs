using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class EventHelpers
{
    public static bool PointerOverUI() => EventSystem.current.IsPointerOverGameObject(); 
    public static void TransferDrag(PointerEventData eventData, GameObject go)
    {
        eventData.pointerDrag = go;
        ExecuteEvents.Execute(go, eventData, ExecuteEvents.initializePotentialDrag);
    }
}