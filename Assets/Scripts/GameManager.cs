using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public uint battles;
    public static GameManager Instance;
    public GameObject SelectedCreature;
    public GameObject HoveringCreature;
    public GameObject LockedDescriptionCreature;
    public bool CanBePlaced;
    public bool SuppressShopTransactions;
    public UnityEvent<bool> PlaceCreature = new();
    private int descriptionOpenedFrame = -1;
    private bool suppressNextCardDescriptionClick;

    private void Update()
    {
        if (Pointer.current == null || !Pointer.current.press.wasReleasedThisFrame)
            return;

        if (LockedDescriptionCreature != null && descriptionOpenedFrame != Time.frameCount)
            LockedDescriptionCreature = null;
    }

    public void OpenDescriptionCreature(GameObject creature)
    {
        LockedDescriptionCreature = creature;
        descriptionOpenedFrame = Time.frameCount;
    }

    public void CloseDescriptionForPointerDown()
    {
        if (LockedDescriptionCreature == null)
            return;

        LockedDescriptionCreature = null;
        suppressNextCardDescriptionClick = true;
    }

    public bool ConsumeSuppressedCardDescriptionClick()
    {
        var suppress = suppressNextCardDescriptionClick;
        suppressNextCardDescriptionClick = false;
        return suppress;
    }

    public void ClearDescriptionCreature(GameObject creature)
    {
        if (LockedDescriptionCreature == creature)
            LockedDescriptionCreature = null;
    }

    public static void ResetRunState()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.DestroyForNewRun();

        if (BoardManager.Instance != null)
            BoardManager.Instance.DestroyForNewRun();

        if (MapGenerator.Instance != null)
            MapGenerator.Instance.DestroyForNewRun();

        if (BattleManager.Instance != null)
            BattleManager.Instance.DestroyForNewRun();

        if (Instance != null)
        {
            Instance.battles = 0;
            Instance.SelectedCreature = null;
            Instance.HoveringCreature = null;
            Instance.LockedDescriptionCreature = null;
            Instance.CanBePlaced = false;
            Instance.SuppressShopTransactions = false;
            Instance.suppressNextCardDescriptionClick = false;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
