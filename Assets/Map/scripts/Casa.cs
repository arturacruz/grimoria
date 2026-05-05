using System.Collections.Generic;
using UnityEngine;

public enum CategoriaCasa
{
    Combate,
    Shop
}

public class Casa : MonoBehaviour
{
    public List<Casa> lista_casa = new List<Casa>();

    public CategoriaCasa Type = CategoriaCasa.Combate;
}