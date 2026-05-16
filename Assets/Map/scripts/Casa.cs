using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CategoriaCasa
{
    Vizitada,
    Combate,
    Shop,
    Boss,
    Inicio
}

public class Casa : MonoBehaviour
{
    public List<Casa> lista_casa = new List<Casa>();

    public CategoriaCasa tipo_casa;
}