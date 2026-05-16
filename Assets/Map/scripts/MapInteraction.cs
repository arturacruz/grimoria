using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class MapInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Vector3 originalScale;
    [SerializeField] private SpriteRenderer marked;
    public List<Casa> casasVisitadas = new();
    
    private void Start()
    {
        originalScale = transform.localScale;
        marked.enabled = false;
        marked.transform.localScale = Vector3.one * 0.4f;
    }

    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     bool isMapScene = scene.name == "MapScene";

    //     if (!isMapScene) return;

    //     foreach (Casa casa in casasVisitadas)
    //     {
    //         print(casa);
    //         if (casa == null) continue;

    //         var mark = casa.GetComponentInChildren<SpriteRenderer>(true);
    //         if (mark != null)
    //             mark.enabled = true;

    //         var sr = casa.GetComponent<SpriteRenderer>();
    //         if (sr != null)
    //             sr.color = Color.white;
    //     }
    // }

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
        if (!MapGenerator.Instance.active)
        {
            return;
        }

        Casa casa_click = gameObject.GetComponent<Casa>();
        if (MapGenerator.casa_atual.lista_casa.Contains(casa_click) && casa_click.tipo_casa != CategoriaCasa.Visitada) {
            CategoriaCasa tipo_casa = casa_click.tipo_casa;
            if (tipo_casa == CategoriaCasa.Combate || tipo_casa == CategoriaCasa.Boss)
            {
                SceneManager.LoadScene("CombatScene");
            }

            else if (tipo_casa == CategoriaCasa.Shop)
            {
                SceneManager.LoadScene("StoreScene");
            }
            casa_click.tipo_casa = CategoriaCasa.Visitada;
            casa_click.gameObject.name = "Visitada";
            SpriteRenderer sprite_casa = casa_click.GetComponent<SpriteRenderer>();
            sprite_casa.color = Color.white;
            var mark = casa_click.GetComponentInChildren<SpriteRenderer>(true);
            if (mark != null)
            {
                mark.enabled = true;
            }

            MapGenerator.casa_atual = casa_click;
            MapGenerator.Player.transform.position = casa_click.transform.position;

            if (!casasVisitadas.Contains(casa_click)){
                casasVisitadas.Add(casa_click);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Casa>().transform.localScale = originalScale;
    }
}