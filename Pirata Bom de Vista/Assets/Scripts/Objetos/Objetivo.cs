using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TipoObjetivo {
    CUTSCENE,
    LUGAR,
    COMBATE
}

[Serializable]
public class Objetivo {
    public string Titulo = "";
    public string Descricao = "";
    public TipoObjetivo Tipo = TipoObjetivo.CUTSCENE;
}

[Serializable]
public class ListaObjetivos{
    public List<Objetivo> TodosObjetivos;

    public Objetivo GetObjetivo(int indice) {
        return TodosObjetivos[indice];
    }
}

[Serializable]
public class FaseAtual {
    public int fase = 0;
    public int indice = 0;
}
