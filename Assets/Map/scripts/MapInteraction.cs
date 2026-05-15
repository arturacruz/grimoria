using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class MapInteraction :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler
{
    private Vector3 originalScale;

    [SerializeField]
    private SpriteRenderer marked;

    private void Awake()
    {
        marked.enabled = false;
    }

    private void Start()
    {
        originalScale =
            transform.localScale;

        marked.transform.localScale =
            Vector3.one * 0.4f;
    }

    public void SetMarked(bool value)
    {
        marked.enabled = value;
    }

    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        Casa casa =
            gameObject.GetComponent<Casa>();

        if (casa.tipo_casa ==
            CategoriaCasa.Boss)
        {
            casa.transform.localScale =
                Vector3.one * 1f;
        }
        else
        {
            casa.transform.localScale =
                Vector3.one * 0.677f;
        }
    }

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        Casa casa_click =
            gameObject.GetComponent<Casa>();

        if ( GameManager.casa_atual.lista_casa.Contains(casa_click) && !casa_click.visitada)
        {
            CategoriaCasa tipo_casa = casa_click.tipo_casa;
            if (tipo_casa == CategoriaCasa.Combate || tipo_casa == CategoriaCasa.Boss)
            {
                SceneManager.LoadScene("CombatScene");
            }

            else if (tipo_casa == CategoriaCasa.Shop)
            {
                SceneManager.LoadScene("StoreScene");
            }

            casa_click.visitada = true;

            casa_click.gameObject.name =
                "Vizitada";

            SpriteRenderer sprite_casa =
                casa_click.GetComponent<SpriteRenderer>();

            sprite_casa.color =
                Color.white;

            marked.enabled = true;

            GameManager.casa_atual =
                casa_click;

            MapGenerator.Player.transform.position =
                new Vector3(
                    casa_click.transform.position.x,
                    casa_click.transform.position.y,
                    -1f
                );

            MapGenerator.Instance.SaveMap();
        }
    }

    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        gameObject.GetComponent<Casa>()
            .transform.localScale =
                originalScale;
    }
}