using UnityEngine;

namespace Store.Scripts
{
    public class HoverGlow : MonoBehaviour
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
            if (glowObject != null)
                glowObject.SetActive(true);

            Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
        }

        void OnMouseExit()
        {
            if (glowObject != null)
                glowObject.SetActive(false);

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}