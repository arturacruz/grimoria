using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool isMouseOver = false;
    private bool selected = false;
    [SerializeField] private GameObject parent;

    private void Update()
    {
        if (!selected)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        
        // Unity fucking converts the position to -10 since that is the camera Z.
        mousePos.z = 0;

        parent.transform.position = mousePos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    private void ChangeChildrenSortingLayer(bool above)
    {
        foreach (var rend in parent.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (above)
                rend.sortingLayerID = SortingLayer.NameToID("SelectedCards");
            else
                rend.sortingLayerID = SortingLayer.NameToID("Cards");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selected = isMouseOver && !selected;

        if (isMouseOver)
        {
            if (selected)
                GridComponent.CreatureSelected.Invoke(parent, true);
            else
                GridComponent.CreatureSelected.Invoke(parent, false);
            ChangeChildrenSortingLayer(selected);
        }
        else GridComponent.CreatureSelected.Invoke(null, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}