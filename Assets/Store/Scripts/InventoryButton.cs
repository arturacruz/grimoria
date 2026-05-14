using UnityEngine;

namespace Store.Scripts
{
    public class InventoryButton : MonoBehaviour
    {
        [SerializeField] private CameraInventoryMover inventory;

        private void OnMouseDown()
        {
            inventory.Toggle();
        }
    }
}