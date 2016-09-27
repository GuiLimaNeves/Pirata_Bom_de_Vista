using UnityEngine;
using System.Collections;

public class ObjetivosContent : MonoBehaviour {
    public static ObjetivosContent instance;


    [SerializeField]
    public ListaObjetivos[] fases;

	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public Objetivo GetObjetivoAtual(FaseAtual faseAtual) {
        return CopiaObjetivo(fases[faseAtual.fase].GetObjetivo(faseAtual.indice));
    }

    public Objetivo CopiaObjetivo(Objetivo original) {
        Objetivo temporario = new Objetivo();
        temporario.Titulo = original.Titulo;
        temporario.Descricao = original.Descricao;
        temporario.Tipo = original.Tipo;

        return temporario;
    }

    public int GetNumeroObjetivos(int temp) {
        return fases[temp].TodosObjetivos.Count;
    }

    public int GetNumeroFases() {
        return fases.Length;
    }
}
