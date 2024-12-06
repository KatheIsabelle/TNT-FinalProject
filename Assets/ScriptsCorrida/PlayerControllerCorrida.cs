using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerCorrida : MonoBehaviour
{
    private Vector3 direcao;
    private Animator animator;
    public float velocidade = 5;
    private Rigidbody rb;
    public InterfaceControllerCorrida scriptInterfaceControllerCorrida;
    public float distanciaFrente = 5f;
    private bool caiu = false;
    private int latasPerdidas = 3;
    private bool imune = false; // Controla a imunidade
    public Transform pontoFinal; // Posição para onde o jogador irá ao cruzar a linha de chegada
    public Transform cameraFinal; // Posição final da câmera
    private bool cruzouLinhaChegada = false; // Indica se o jogador já cruzou a linha

    public float forcaPulo = 7f; // Força do pulo
    private bool estaNoChao = true; // Verifica se o jogador está no chão
    public LayerMask camadaChao; // Camada do chão para detectar colisão
    public Transform detectorChao; // Ponto para detectar o chão
    public float raioDeteccao = 0.2f; // Raio de detecção do chão

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (cruzouLinhaChegada) return; // Impede novos movimentos se já terminou

        Mover();

        if (transform.position.y < -4 && !caiu)
        {
            caiu = true;
            Reposicionar();
        }

        VerificarChao();

        if (estaNoChao && (Input.GetButtonDown("Jump") || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)))
        {
            Pular();
        }
    }

    private void VerificarChao()
    {
        // Detecta se o jogador está no chão usando Physics.CheckSphere
        estaNoChao = Physics.CheckSphere(detectorChao.position, raioDeteccao, camadaChao);
        animator.SetBool("NoChao", estaNoChao); // Atualiza a animação de "no chão"
    }

    private void Pular()
    {
        rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
        animator.SetTrigger("Pular"); // Ativa a animação de pulo
    }

    private void Reposicionar()
    {
        PerdeLatas(latasPerdidas);
        Transform cameraTransform = Camera.main.transform;

        Vector3 posicaoNaFrente = cameraTransform.position + cameraTransform.forward * distanciaFrente;
        posicaoNaFrente.y = 0;

        // Reposicionar o personagem em vez de instanciar
        transform.position = posicaoNaFrente;
        transform.rotation = Quaternion.identity;

        caiu = false; // Permite que ele caia novamente no futuro
    }

    private void Mover()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direcao = new Vector3(horizontal, 0, vertical);

        rb.MovePosition(rb.position + direcao.normalized * velocidade * Time.deltaTime);

        if (direcao != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direcao), Time.deltaTime * 10);
        }

        animator.SetBool("Move", direcao != Vector3.zero);
    }

    public void ColetarLatas()
    {
        scriptInterfaceControllerCorrida.AumentarQuantidadeDeLatasColetadas();
    }

    public void PerdeLatas(float latasPerdidas)
    {
        if (!imune)
        {
            scriptInterfaceControllerCorrida.DiminuirQuantidadeLatasColetadas(latasPerdidas);
            StartCoroutine(Invencibilidade());
        }
    }

    private IEnumerator Invencibilidade()
    {
        imune = true; // Ativa a imunidade
        velocidade = 0; // Para o jogador
        float tempoImune = 1f; // Tempo de invencibilidade (em segundos)
        float intervaloPiscar = 0.2f; // Intervalo entre os piscamentos

        // Efeito de piscamento alternando a visibilidade do modelo
        for (float tempo = 0; tempo < tempoImune; tempo += intervaloPiscar)
        {
            animator.SetTrigger("Atingido");
            yield return new WaitForSeconds(intervaloPiscar);
        }

        imune = false;
        velocidade = 5; // Restaura a velocidade do jogador
    }

    public void CruzarLinhaDeChegada()
    {
        cruzouLinhaChegada = true; // Bloqueia o movimento normal
        animator.SetBool("Move", true); // Ativa a animação de andar (se necessário)
        StartCoroutine(AndarAtePontoFinal());
    }


    private IEnumerator AndarAtePontoFinal()
    {
        float velocidadeFinal = 2f; // Velocidade do jogador ao final
        Vector3 direcao = (pontoFinal.position - transform.position).normalized;

        while (Vector3.Distance(transform.position, pontoFinal.position) > 0.1f)
        {
            transform.position += direcao * velocidadeFinal * Time.deltaTime;
            yield return null;
        }

        // Ajusta a posição exata e orientação
        transform.position = pontoFinal.position;
        transform.LookAt(cameraFinal.position); // Faz o jogador olhar para a câmera
        animator.SetBool("Move", false); // Para a animação de andar

        // Finaliza o jogo
        Camera.main.GetComponent<CameraControllerCorrida>().PosicionarCameraFinal();

        // Exibe mensagem de vitória (opcional)
        Debug.Log("Você terminou a corrida!");
    }

}
