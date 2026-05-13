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
        CategoriaCasa tipo_casa = gameObject.GetComponent<Casa>().tipo_casa;
        if (tipo_casa == CategoriaCasa.Combate || tipo_casa == CategoriaCasa.Boss)
        {
            SceneManager.LoadScene("CombatScene");
        }

        else if (tipo_casa == CategoriaCasa.Shop)
        {
            SceneManager.LoadScene("StoreScene");
        }
        print("teste");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}