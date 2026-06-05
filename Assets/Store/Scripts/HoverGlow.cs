using UnityEngine;
using UnityEngine.EventSystems;

namespace Store.Scripts
{
    public class HoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject glowObject;
        public Texture2D handCursor;

        void Start()
        {
            if (glowObject != null)
                glowObject.SetActive(false);
        }

        void OnMouseEnter()
        {
            ShowGlow();
        }

        void OnMouseExit()
        {
            HideGlow();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowGlow();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideGlow();
        }

        private void ShowGlow()
        {
            if (glowObject != null)
                glowObject.SetActive(true);

            if (handCursor != null)
                Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
        }

        private void HideGlow()
        {
            if (glowObject != null)
                glowObject.SetActive(false);

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
