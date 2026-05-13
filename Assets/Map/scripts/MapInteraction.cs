using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class MapInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool isMouseOver = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }
    public void OnPointerDown(PointerEventData eventData) {
        Casa casa_click = gameObject.GetComponent<Casa>();

        if (MapGenerator.casa_atual.lista_casa.Contains(casa_click)) {
            CategoriaCasa tipo_casa = casa_click.tipo_casa;
            
            if (tipo_casa == CategoriaCasa.Combate || tipo_casa == CategoriaCasa.Boss)
            {
                SceneManager.LoadScene("CombatScene");
            }

            else if (tipo_casa == CategoriaCasa.Shop)
            {
                SceneManager.LoadScene("StoreScene");
            }

            MapGenerator.casa_atual = casa_click;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}