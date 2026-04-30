using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool isMouseOver = false;
    private bool selected = false;

    private void Update()
    {
        if (!selected)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        
        // Unity fucking converts the position to -10 since that is the camera Z.
        mousePos.z = 0;

        transform.position = mousePos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMouseOver && !selected)
            selected = true;
        else 
            selected = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}