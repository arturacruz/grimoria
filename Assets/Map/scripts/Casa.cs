using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CategoriaCasa
{
    Combate,
    Shop,
    Boss,
    
    Inicio
}

public class Casa : MonoBehaviour
{
    public List<Casa> lista_casa = new List<Casa>();

    public CategoriaCasa tipo_casa;

    private void Awake()
    {
        int x = Random.Range(1, 17);
        if (x == 1)
        {
            tipo_casa = CategoriaCasa.Shop;
        }
        else
        {
            tipo_casa = CategoriaCasa.Combate;
        }
     }
}