using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DragComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private const float ClickMoveThresholdPixels = 12f;

    [SerializeField] private GameObject parent;
    private Camera camera;
    private Vector3 originalPosition;
    private bool originalWasShopCard;
    private uint originalShopPrice;
    private Vector2 pointerDownPosition;
    private bool movedPastClickThreshold;
    private GridComponent sourceGrid;

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
        var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
            camera = mainCamera.GetComponent<Camera>();
    }

    private bool TryGetPointerWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;
        if (Pointer.current == null || camera == null)
            return false;

        worldPosition = camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        worldPosition.z = 0;
        return true;
    }

    private void Update()
    {
        if (!isSelected || !isPlayer)
            return;

        if (Pointer.current == null)
            return;

        if (!Pointer.current.press.isPressed)
        {
            FinishDrag();
            return;
        }

        var pointerPosition = Pointer.current.position.ReadValue();
        if (!movedPastClickThreshold && Vector2.Distance(pointerDownPosition, pointerPosition) > ClickMoveThresholdPixels)
        {
            movedPastClickThreshold = true;
            GameManager.Instance.ClearDescriptionCreature(parent);
        }

        if (!TryGetPointerWorldPosition(out var pointerWorldPosition))
            return;
        canBePlaced = false;

        parent.transform.position = pointerWorldPosition;
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
        GameManager.Instance.HoveringCreature = parent;
        GameManager.Instance.CloseDescriptionForPointerDown();
        pointerDownPosition = eventData.position;
        movedPastClickThreshold = false;

        if (otherCreatureSelected
            || !isPlayer
            || (BattleManager.Instance != null && BattleManager.Instance.battleOngoing))
            return;

        var creature = parent.GetComponent<Creature>();
        sourceGrid = FindCurrentGrid(creature);
        canBePlaced = false;

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

        originalPosition = parent.transform.position;
        originalWasShopCard = isShopCard;
        originalShopPrice = isShopCard ? InventoryManager.Instance.GetPrice(creature) : 0;
        isSelected = true;

        GameManager.Instance.SuppressShopTransactions = originalWasShopCard;
        GameManager.Instance.PlaceCreature.Invoke(false);
        GameManager.Instance.SuppressShopTransactions = false;

        ChangeChildrenSortingLayer(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var wasClick = !movedPastClickThreshold 
                       && Vector2.Distance(pointerDownPosition, eventData.position) <= ClickMoveThresholdPixels;
        var suppressDescriptionClick = GameManager.Instance.ConsumeSuppressedCardDescriptionClick();

        if (isSelected)
            FinishDrag();

        if (wasClick && !suppressDescriptionClick)
            GameManager.Instance.OpenDescriptionCreature(parent);
    }

    private void FinishDrag()
    {
        var creature = parent.GetComponent<Creature>();
        var dropped = canBePlaced;

        if (!dropped)
            parent.transform.position = originalPosition;

        GameManager.Instance.SuppressShopTransactions = originalWasShopCard;
        GameManager.Instance.PlaceCreature.Invoke(true);
        GameManager.Instance.SuppressShopTransactions = false;

        if (!IsRegisteredInAnyGrid(creature))
        {
            GameManager.Instance.SuppressShopTransactions = originalWasShopCard;
            if (dropped)
            {
                if (!TryPlaceInAnyGrid(creature, parent.transform.position))
                {
                    parent.transform.position = originalPosition;
                    sourceGrid?.TryPlaceExistingCreatureAtWorldPosition(creature, originalPosition);
                }
            }
            else
            {
                sourceGrid?.TryPlaceExistingCreatureAtWorldPosition(creature, originalPosition);
            }
            GameManager.Instance.SuppressShopTransactions = false;
        }

        if (dropped && originalWasShopCard && InventoryManager.Instance != null)
        {
            var boughtIntoInventory = InventoryManager.Instance.Contains(creature);
            var boughtIntoBoard = BoardManager.Instance != null && BoardManager.Instance.Contains(creature);
            if (boughtIntoInventory || boughtIntoBoard)
                InventoryManager.Instance.TrySpend(originalShopPrice);
        }

        isSelected = false;
        canBePlaced = false;
        originalWasShopCard = false;
        originalShopPrice = 0;
        sourceGrid = null;
        ChangeChildrenSortingLayer(false);
    }

    private GridComponent FindCurrentGrid(Creature creature)
    {
        var grids = FindObjectsByType<GridComponent>(FindObjectsSortMode.None);
        foreach (var grid in grids)
        {
            if (grid.ContainsCreature(creature))
                return grid;
        }

        return null;
    }

    private bool IsRegisteredInAnyGrid(Creature creature)
    {
        var grids = FindObjectsByType<GridComponent>(FindObjectsSortMode.None);
        foreach (var grid in grids)
        {
            if (grid.ContainsCreature(creature))
                return true;
        }

        return false;
    }

    private bool TryPlaceInAnyGrid(Creature creature, Vector3 position)
    {
        var grids = FindObjectsByType<GridComponent>(FindObjectsSortMode.None);
        foreach (var grid in grids)
        {
            if (grid.TryPlaceExistingCreatureAtWorldPosition(creature, position))
                return true;
        }

        return false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.HoveringCreature = null;
    }
}
