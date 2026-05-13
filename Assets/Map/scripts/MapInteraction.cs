using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class MapInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Casa casa = gameObject.GetComponent<Casa>();
        if (casa.tipo_casa == CategoriaCasa.Boss)
        {
            casa.transform.localScale = Vector3.one * 1f;
        }
        else
        {
        casa.transform.localScale = Vector3.one * 0.677f;
            
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        Casa casa_click = gameObject.GetComponent<Casa>();
        if (MapGenerator.casa_atual.lista_casa.Contains(casa_click) && casa_click.tipo_casa != CategoriaCasa.Vizitada) {
            CategoriaCasa tipo_casa = casa_click.tipo_casa;
            if (tipo_casa == CategoriaCasa.Combate || tipo_casa == CategoriaCasa.Boss)
            {
                SceneManager.LoadScene("CombatScene");
            }

            else if (tipo_casa == CategoriaCasa.Shop)
            {
                SceneManager.LoadScene("StoreScene");
            }
            casa_click.tipo_casa = CategoriaCasa.Vizitada;
            casa_click.gameObject.name = "Vizitada";
            SpriteRenderer sprite_casa = casa_click.GetComponent<SpriteRenderer>();
            sprite_casa.color = Color.white;

            MapGenerator.casa_atual = casa_click;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Casa>().transform.localScale = originalScale;
    }
}