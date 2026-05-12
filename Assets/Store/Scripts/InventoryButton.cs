using UnityEngine;

namespace Store.Scripts
{
    public class InventoryButton : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;

        private void OnMouseDown()
        {
            inventory.Toggle();
        }
    }
}