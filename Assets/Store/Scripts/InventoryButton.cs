using UnityEngine;
using UnityEngine.EventSystems;

namespace Store.Scripts
{
    public class InventoryButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private CameraInventoryMover inventory;

        private void OnMouseDown()
        {
            ToggleInventory();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ToggleInventory();
        }

        private void ToggleInventory()
        {
            if (inventory != null)
                inventory.Toggle();
        }
    }
}
