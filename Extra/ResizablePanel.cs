using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{

    public GameObject leftResizeVisualizer;
    public GameObject bottomResizeVisualizer;
    
    public Vector2 minSize;
    public Vector2 maxSize;
    public float leftEdgeThreshold = 5;
    public float bottomEdgeThreshold = 5;
    [FormerlySerializedAs("edgeThreshold")] public float cornerEdgeThreshold = 10f; 

    private RectTransform rectTransform;
    private Vector2 currentPointerPosition;
    private Vector2 previousPointerPosition;
    private bool resizeWidth; // X ekseninde genişleme
    private bool resizeHeight; // Y ekseninde genişleme
    private bool startedToResize;

    void Awake () {
        rectTransform = GetComponent<RectTransform>();  // Kendini hedef al
    }

    private void Update()
    {
        if (startedToResize) return;
        leftResizeVisualizer.SetActive(resizeWidth);
        bottomResizeVisualizer.SetActive(resizeHeight);

        if (!UIHelpers.GetComponentInParentUnderPointer(GetType()))
        {
            resizeWidth = false;
            resizeHeight = false;
            return;
        }
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, 
            Camera.current, out previousPointerPosition);
        
        Vector2 panelMin = rectTransform.rect.min;

        float leftDist = Mathf.Abs(previousPointerPosition.x - panelMin.x);
        float bottomDist = Mathf.Abs(previousPointerPosition.y - panelMin.y);
        
        if (leftDist < cornerEdgeThreshold && bottomDist < cornerEdgeThreshold)
        {
            resizeWidth = true;
            resizeHeight = true;
        }
        else
        {
            resizeWidth = leftDist < leftEdgeThreshold;
            resizeHeight = bottomDist < bottomEdgeThreshold;
        }
    }

    public void OnPointerDown (PointerEventData data) {
        rectTransform.SetAsLastSibling();
        startedToResize = true;
    }

    public void OnDrag (PointerEventData data) {
        if (rectTransform == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
        Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

        Vector2 newSizeDelta = rectTransform.sizeDelta;

        if (resizeWidth) {
            newSizeDelta.x -= resizeValue.x;
        }
        if (resizeHeight) {
            newSizeDelta.y -= resizeValue.y;
        }

        // Boyutları sınırlarla sınırlama
        newSizeDelta.x = Mathf.Clamp(newSizeDelta.x, minSize.x, maxSize.x);
        newSizeDelta.y = Mathf.Clamp(newSizeDelta.y, minSize.y, maxSize.y);

        rectTransform.sizeDelta = newSizeDelta;

        previousPointerPosition = currentPointerPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        startedToResize = false;
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        startedToResize = false;
        
    }
}
