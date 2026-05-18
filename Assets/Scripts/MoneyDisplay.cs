using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Update()
    {
        if (text == null)
            return;

        if (InventoryManager.Instance == null)
        {
            text.text = "Gold: 0";
            return;
        }

        text.text = $"Gold: {InventoryManager.Instance.money}";
    }
}