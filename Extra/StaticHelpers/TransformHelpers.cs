using UnityEngine;

public static class TransformHelpers
{
    public static T GetComponentInPrimaryChildren<T>(this GameObject gameObject) where T : Component
    {
        return GetComponentInPrimaryChildren<T>(gameObject.transform);
    }
    
    public static T GetComponentInPrimaryChildren<T>(this Transform transform) where T : Component
    {
        foreach (Transform child in transform)
        {
            var comp = child.GetComponent<T>();
            if (comp != null) return comp;
        }

        return null;
    }
    
    public static Vector2 GetAnchoredPositionRelativeTo(this RectTransform rectTransform, RectTransform reference)
    {
        var startPos = (Vector2) reference.position - reference.rect.size / 2;
        Vector2 anchorOffset = (rectTransform.anchorMax + rectTransform.anchorMin) * 0.5f * reference.rect.size;
        var anchorCenterPoint = startPos + anchorOffset;

        var diff = (Vector2)rectTransform.position - anchorCenterPoint;

        return diff;
    }
    
    public static void SetAnchoredPositionRelativeTo(this RectTransform rectTransform, RectTransform reference, Vector2 relativePosition)
    {
        var startPos = (Vector2) reference.position - reference.rect.size / 2;
        Vector2 anchorOffset = (rectTransform.anchorMax + rectTransform.anchorMin) * 0.5f * reference.rect.size;
        var anchorCenterPoint = startPos + anchorOffset;
        
        Vector2 newPosition = anchorCenterPoint + relativePosition;

        rectTransform.position = newPosition;
    }

}