using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool isMouseOver = false;
    private bool selected = false;
    
    // For cases of overlapping
    private bool otherCreatureSelected = false;
    [SerializeField] private GameObject parent;

    private void Start()
    {
        GridComponent.CreatureSelected.AddListener(OnCreatureSelected);
    }
    
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
        Debug.Log($"isMouseOver {isMouseOver}, selected: {selected}, can be placed {parent.GetComponent<CreatureComponent>().canBePlaced}");
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
        
        var component = parent.GetComponent<CreatureComponent>();
        if (isMouseOver)
        {
            if (!component.canBePlaced && selected)
                return;

            selected = !selected;
            GridComponent.CreatureSelected.Invoke(parent, selected);
        }
        else
        {
            component.canBePlaced = false;
            selected = false;
            GridComponent.CreatureSelected.Invoke(null, false);
        }
        ChangeChildrenSortingLayer(selected);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    private void OnCreatureSelected(GameObject creature, bool selection)
    {
        otherCreatureSelected = creature != parent && selection;
    }
}