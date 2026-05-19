using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DragComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool isMouseOver => GameManager.Instance.HoveringCreature == parent;
    [SerializeField] private GameObject parent;
    private Camera camera;

    private bool isPlayer => parent.GetComponent<Creature>().playerSide;

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

    private bool canBePlaced
    {
        get => GameManager.Instance.CanBePlaced;
        set => GameManager.Instance.CanBePlaced = value;
    }

    private void Start()
    {

        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }


    private void Update()
    {
        if (!isSelected || !isPlayer)
            return;

        if (camera == null)
            return;
        canBePlaced = false;

        var mousePos = camera.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        
        // Unity fucking converts the position to -10 since that is the camera Z.
        mousePos.z = 0;

        parent.transform.position = mousePos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.HoveringCreature = parent;
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
        if (otherCreatureSelected
            || !isPlayer
            || (BattleManager.Instance != null && BattleManager.Instance.battleOngoing))
            return;

        if (!isMouseOver)
        {
            isSelected = false;
            ChangeChildrenSortingLayer(false);
            return;
        }

        var creature = parent.GetComponent<Creature>();

        bool isStoreScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "StoreScene";
        bool isShopCard = isStoreScene
                          && InventoryManager.Instance != null
                          && !InventoryManager.Instance.Contains(creature);

        // Só bloqueia quando está tentando PEGAR a carta da loja.
        if (!isSelected && isShopCard)
        {
            uint price = InventoryManager.Instance.GetPrice(creature);
            if (InventoryManager.Instance.money < price)
                return;
        }

        if (!canBePlaced && isSelected)
            return;

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

        ChangeChildrenSortingLayer(isSelected);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.HoveringCreature = null;
    }
}