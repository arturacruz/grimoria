using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool isMouseOver;
    [SerializeField] private GameObject parent;

    private bool isSelected
    {
        get => GameManager.Instance.SelectedCreature == parent;
        set
        {
            if (!value)
                GameManager.Instance.SelectedCreature = null;
            else
                GameManager.Instance.SelectedCreature = parent;
        }
    }

    private bool otherCreatureSelected => GameManager.Instance.SelectedCreature != null && 
                                          GameManager.Instance.SelectedCreature != parent;

    private bool canBePlaced => GameManager.Instance.CanBePlaced;
    

    private void Update()
    {
        if (!isSelected)
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
        if (otherCreatureSelected)
            return;
        
        if (isMouseOver)
        {
            if (!canBePlaced && isSelected)
                return;

            // Order of invoking event has to be this, or else the creature will be null
            if (isSelected)
            {
                GameManager.Instance.PlaceCreature.Invoke(true);
                isSelected = false;
            }
            else
            {
                isSelected = true;
                GameManager.Instance.PlaceCreature.Invoke(false);
            }
        }
        else
            isSelected = false;
        ChangeChildrenSortingLayer(isSelected);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}