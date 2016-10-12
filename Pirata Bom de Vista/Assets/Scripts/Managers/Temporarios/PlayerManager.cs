using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState {
    MOVIMENTACAO,
    COMBATE,
    CUTSCENE,
}

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;

    [HideInInspector]
    public PlayerState playerState;
    public AudioClip[] falas;

    [SerializeField]
    public List<CorteAnimacao> listaAnimacao = new List<CorteAnimacao>();


    private Animator animator;
    private int indiceNome = 1;
    private CorteAnimacao corteAtual;
    private AudioSource _audioSource;
    private Transform myTransform;

    //TP_CONTROLLER

    public static CharacterController CharacterController;


    // Use this for initialization
    void Start () {
        instance = this;
        animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        myTransform = this.transform;

        CharacterController = GetComponent("CharacterController") as CharacterController;
        CameraManager.UseExistingOrCreateNewMainCamera();
    }
	
	// Update is called once per frame
	void Update () {



        switch (playerState)
        {
            case PlayerState.MOVIMENTACAO:

                if (Camera.main == null)
                    return;
                GetLocomotionInput();


                HandleActionInput();

                PlayerController.instance.UpdateMotor();
                break;
        }


 


    }

    void OnEnable()
    {
        LevelManager.eventoCutscene += PlayAnimacaoCutscene;
    }

    void OnDisable()
    {
        LevelManager.eventoCutscene -= PlayAnimacaoCutscene;
    }

    public void SetPlayerState(PlayerState temp) {
        if (temp == PlayerState.MOVIMENTACAO || temp == PlayerState.COMBATE)
        {
            //PlayerController.instance.SetPermiteInput(true);

            if (temp == PlayerState.COMBATE)
            {
                //PlayerController.instance.SetInputCombate(true);
            }
            else {
                //PlayerController.instance.SetInputCombate(false);
            }
        }
        else {
            //PlayerController.instance.SetPermiteInput(false);
            //PlayerController.instance.SetInputCombate(false);
        }

        //Debug.Log(temp);
        playerState = temp;
    }


    public void PlayAnimacaoCutscene()
    {
        ConteudoAtual indiceAnim = DirectorsManager.instance.GetCorteAtual();


        if (FazParteAnimacao(indiceAnim))
        {
            RuntimeAnimatorController myController = animator.runtimeAnimatorController;
            AnimatorOverrideController anim_OverrideController = new AnimatorOverrideController();
            anim_OverrideController.runtimeAnimatorController = myController;
            AnimationClip clipe = corteAtual.clipeAnim;
            string nome = "";
            //for (int i = 0; i < anim_OverrideController.clips.Length; i++) {
            //Debug.Log(anim_OverrideController.clips[i].overrideClip.name.ToString());
            //  if (anim_OverrideController.clips[i].ToString() != "Idle") {

            //} 
            //}




            if (indiceNome > 1)
            {

                nome = "ct_" + (indiceNome - 1).ToString();
            }
            else
            {
                nome = "ct_" + (indiceNome).ToString();
            }


            //Debug.Log("Nome anterior: " + nome);
            indiceNome++;


            anim_OverrideController[nome] = clipe;
            animator.runtimeAnimatorController = anim_OverrideController;

            if (corteAtual.posicao != Vector3.zero) {
                myTransform.position = corteAtual.posicao;
            }

            if (corteAtual.rotacao != Vector3.zero) {
                myTransform.eulerAngles = corteAtual.rotacao;
            }


            StartCoroutine(AtivaAnimacaoCutscene());

            if (corteAtual.decisivo == true)
            {
                //Debug.Log("Decisivo");
                StartCoroutine(EsperaTerminoAnimacao());
            }
        }
    }

    private IEnumerator AtivaAnimacaoCutscene()
    {
        yield return new WaitForSeconds(DirectorsManager.instance.tempoAguardoCutscenes);
        animator.SetTrigger("Cutscene");
    }

    private IEnumerator EsperaTerminoAnimacao()
    {
        DirectorsManager.instance.IniciarAnimDecisiva();

        //Aguarda o começo
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }

        //Aguarda o termino
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("ct_Anim"))
        {
            yield return null;
        }



        DirectorsManager.instance.AcabarAnimDecisiva();

        yield return 0;
    }



    public bool FazParteAnimacao(ConteudoAtual indice)
    {
        
        for (int i = 0; i < listaAnimacao.Count; i++)
        {
            if (indice.fase == listaAnimacao[i].indiceAnim.fase)
            {
                if (indice.cutscene == listaAnimacao[i].indiceAnim.cutscene) {
                    if (indice.corte == listaAnimacao[i].indiceAnim.corte) {
                        corteAtual = DirectorsManager.instance.CopiaCorteAnimacao(listaAnimacao[i]);
                        return true;
                    }
                }
            }
        }

        //Debug.Log("Faz parte?");
        corteAtual = new CorteAnimacao();
        return false;


    }


    public void AtivaAudio()
    {
        if (DirectorsManager.instance == null) { return; }

        StartCoroutine(TextoLegenda());
        DirectorsManager.instance.ProximaLegenda();
    }


    private IEnumerator TextoLegenda()
    {
        DirectorsManager.instance.ProximaLegenda();
        yield return new WaitForSeconds(0.5f);
        AtivaFala();
    }

    public void AtivaFala()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            return;
        }

        int random = UnityEngine.Random.Range(0, falas.Length);
        _audioSource.clip = falas[random];
        _audioSource.Play();
    }


    void GetLocomotionInput()
    {

        var deadZone = 0.1f;

        PlayerController.instance.VerticalVelocity = PlayerController.instance.MoveVector.y;
        PlayerController.instance.MoveVector = Vector3.zero;

        if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
        {
            PlayerController.instance.MoveVector += new Vector3(0, 0, Input.GetAxis("Vertical"));
        }

        if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
        {
            PlayerController.instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        }
    }

    void HandleActionInput()
    {

        if (Input.GetButton("Jump"))
        {
            Jump();
        }

    }

    void Jump()
    {

        PlayerController.instance.Jump();
    }

    public void AtualizaPosPlayerObjetivo(SaveTransform newTransform) {
        myTransform.position = newTransform.position;
        myTransform.rotation = newTransform.rotation;
        myTransform.localScale = newTransform.localScale;
    }

}
