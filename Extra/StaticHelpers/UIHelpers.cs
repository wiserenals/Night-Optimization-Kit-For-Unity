using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
public static class UIHelpers
{
    private static GraphicRaycaster _raycaster;
    private static EventSystem _eventSystem;
    private static bool _initialized;

    // Referansları yalnızca bir kez başlatır
    private static void Initialize()
    {
        if (_initialized) return;

        _raycaster = MainScript.Instance.mainRaycaster;
        if (_raycaster == null)
        {
            Debug.LogWarning("No GraphicRaycaster assigned, searching on scene...");

            _raycaster = Object.FindObjectsOfType<GraphicRaycaster>()
                .First(x => x.transform != MainScript.Instance.transform);

            if (_raycaster == null) Debug.LogError("No GraphicRaycaster found on scene.");
            else MainScript.Instance.mainRaycaster = _raycaster;
        }

        _eventSystem = EventSystem.current;
        if (_eventSystem == null)
        {
            Debug.LogError("No EventSystem found in the scene.");
        }

        _initialized = true; // İlk başlatma tamamlandı
    }

    public static GameObject GetUIElementUnderPointer()
    {
        Initialize();

        PointerEventData pointerEventData = new PointerEventData(_eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(pointerEventData, results);

        return results.Count > 0
            ? results[0].gameObject
            : null;
    }
    
    public static GameObject GetComponentInParentUnderPointer(Type type)
    {
        GameObject uiElement = GetUIElementUnderPointer();

        if (uiElement == null) return default; // Eğer parent zincirinde T componenti bulunamazsa null döner
        
        Transform currentTransform = uiElement.transform;

        // Parentları dolaşarak T türündeki ilk componenti bulur
        while (currentTransform != null)
        {
            Component component = currentTransform.GetComponent(type);
            if (component != null)
            {
                return component.gameObject;
            }
            currentTransform = currentTransform.parent;
        }

        return default; // Eğer parent zincirinde T componenti bulunamazsa null döner
    }
    
    public static Sprite GetPrefabSpriteByComponent(object obj)
    {
        switch (obj)
        {
            case GameObject go:
                return GetPrefabSprite(go);
            case Component component:
                return GetPrefabSprite(component.gameObject);
        }

        return default;
    }

    public static Sprite GetPrefabSprite(GameObject prefab)
    {
        var snapshotCamera = SnapshotCamera.MakeSnapshotCamera(0);
        Texture2D snapshot = snapshotCamera
            .TakePrefabSnapshot(prefab, Color.clear, 
                new Vector3(0,-1,1), Quaternion.identity,
                Vector3.one / 10f);
        return Sprite.Create(snapshot, 
            new Rect(0, 0, snapshot.width, snapshot.height), 
            new Vector2(0.5f, 0.5f));
    }
    
    public static T GetComponentInParentUnderPointer<T>()
    {
        GameObject uiElement = GetUIElementUnderPointer();

        if (uiElement == null) return default; // Eğer parent zincirinde T componenti bulunamazsa null döner
        
        Transform currentTransform = uiElement.transform;

        // Parentları dolaşarak T türündeki ilk componenti bulur
        while (currentTransform != null)
        {
            T component = currentTransform.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            currentTransform = currentTransform.parent;
        }

        return default; // Eğer parent zincirinde T componenti bulunamazsa null döner
    }

    /*public static void SetOutline(this GameObject gameObject, Color outlineColor, float thickness = 1.0f)
    {
        // Get or add an Outline component
        var outline = gameObject.GetComponentInPrimaryChildren<UIOutline>();
        if (outline == null)
        {
            outline = Object.Instantiate(MainScript.Instance.outlinePrefab, gameObject.transform);
        }

        outline.enabled = true;
        outline.color = outlineColor;
        outline.outlineWidth = thickness;
    }

    public static void RemoveOutline(this GameObject gameObject)
    {
        var outline = gameObject.GetComponentInPrimaryChildren<UIOutline>();
        if (outline) outline.enabled = false;
    }*/
    
    public static void Align(this RectTransform rectTransform, RectAnchor anchor)
    {
        var lastGlobalPosition = rectTransform.position;
        switch (anchor)
        {
            case RectAnchor.Center:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            case RectAnchor.Top:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                break;
            case RectAnchor.Left:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                break;
            case RectAnchor.Right:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                break;
            case RectAnchor.Bottom:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                break;
            case RectAnchor.StretchAllSides:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case RectAnchor.TopLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                break;
            case RectAnchor.TopRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                break;
            case RectAnchor.BottomLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                break;
            case RectAnchor.BottomRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);

                break;
            default:
                Debug.LogError("Unknown RectAnchor value.");
                break;
        }
        rectTransform.position = lastGlobalPosition;
    }
    
}

public enum RectAnchor
{
    Center,
    Top,
    Left,
    Right,
    Bottom,
    StretchAllSides,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
