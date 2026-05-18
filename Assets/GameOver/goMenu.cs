using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class goMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private SpriteRenderer sprite;

    private Color normalColor;
    private Color hoverColor;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        normalColor = sprite.color;

        hoverColor = new Color(
            normalColor.r * 1.5f,
            normalColor.g * 1.5f,
            normalColor.b * 1.5f,
            normalColor.a
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sprite.color = hoverColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sprite.color = normalColor;
    }
}