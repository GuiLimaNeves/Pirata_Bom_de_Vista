using UnityEngine;
using System.Collections;


public class ObjetivosController : MonoBehaviour {

    public static ObjetivosController instance;

    public Objetivo objetivoAtual;
    private FaseAtual faseAtual;

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

        //TODO: usar indice de fase e de objetivo salvo.
        faseAtual = SaveLoad.instance.GetLoadedData().faseAtual;

        objetivoAtual = ObjetivosContent.instance.GetObjetivoAtual(faseAtual);
        AtivaObjetivo();

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void AtivaObjetivo() {
        
        StartCoroutine(AtivaObjtivo_CO());
        
    }

    private IEnumerator AtivaObjtivo_CO() {
        //Debug.Log("Vendo qual é o objetivo");
        while (FadeManager.instance == null) {
            yield return 0;
        }


        //Desativar todos os inputs caso esteja esperando o fade.
        LevelManager.instance.DesativaInput();

        while (FadeManager.instance.fading || SceneLoader.instance.esperandoFade)
        {

            yield return 0;
        }

        if (objetivoAtual.Tipo == TipoObjetivo.CUTSCENE)
        {
            PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);

            if (FadeManager.instance != null)
            {
                while (FadeManager.instance.fading || SceneLoader.instance.esperandoFade)
                {
                    yield return 0;
                }
            }

            yield return new WaitForSeconds(1);

            UIManager.instance.DesativaTodaUI();

            while (UIManager.instance.alterandoUI)
            {
                yield return null;
            }


            LevelManager.instance.AtivaCutscene();
        }
        else if (objetivoAtual.Tipo == TipoObjetivo.LUGAR)
        {
            while (DirectorsManager.instance == null)
            {
                yield return null;
            }

            while (DirectorsManager.instance.alterando)
            {
                yield return null;
            }


            yield return new WaitForSeconds(0.6f);

            //Debug.Log("Lugar??");
            //TODO: Enquanto não mostrar objetivo, não movimentar.
            UIManager.instance.AtivaTituloObjetivo(objetivoAtual.Titulo);
            LevelManager.instance.AtivaInput();
        }
        else if (objetivoAtual.Tipo == TipoObjetivo.COMBATE)
        {
            PlayerManager.instance.SetPlayerState(PlayerState.COMBATE);
        }


        yield return null;
    }


    public void ProximoObjetivo() {

        if (faseAtual.indice < ObjetivosContent.instance.GetNumeroObjetivos(faseAtual.fase) - 1)
        {
            faseAtual.indice++;
            Objetivo objetivoTemp = ObjetivosContent.instance.CopiaObjetivo(objetivoAtual);
            AtualizaObjetivoAtual();

            if (objetivoTemp.Tipo == TipoObjetivo.CUTSCENE && objetivoAtual.Tipo != TipoObjetivo.CUTSCENE) {
                DirectorsManager.instance.DesativaDiretor();
            }
            AtivaObjetivo();
        }
        else if (faseAtual.fase < ObjetivosContent.instance.GetNumeroFases() - 1)
        {
            faseAtual.fase++;
            AtualizaObjetivoAtual();
            AtivaObjetivo();
        }
        else {
            //PlayerManager.instance.SetPlayerState(PlayerState.CUTSCENE);
            //LevelManager.instance.DesativaInput();
            Debug.Log("Fim de jogo!!");
        }

        SaveLoad.instance.Save();

    }

    public void AtualizaObjetivoAtual() {
        objetivoAtual = ObjetivosContent.instance.GetObjetivoAtual(faseAtual);
    }

    public FaseAtual GetFaseAtual() {
        FaseAtual temp = new FaseAtual();
        temp.fase = faseAtual.fase;
        temp.indice = faseAtual.indice;
        return temp;
    }
}
