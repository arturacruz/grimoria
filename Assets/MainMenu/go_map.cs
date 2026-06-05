using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class go_map : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private bool mouse = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse = true;
        // print("Teste");
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        GameManager.ResetRunState();
        SceneManager.LoadScene("MapScene");
        // print("Teste 777");
        // Debug.Log("CLICK");
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse = false;
        // print("Teste 333");
        
        
    }
}
