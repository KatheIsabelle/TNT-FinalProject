using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControllerCorridaCaminhoEnergizado : MonoBehaviour {
    public int playerID;
    public float velocidade = 5;
    public float forcaPulo = 7f; // Força do pulo
    public LayerMask camadaChao; // Camada do chão para detectar colisão
    public Transform detectorChao; // Ponto para detectar o chão
    public float raioDeteccao = 0.2f; // Raio de detecção do chão

    public Text textoPonto;
    public Text textoPerdeuLatas;

    public Transform pontoFinal; // Posição para onde o jogador irá ao cruzar a linha de chegada
    public Transform cameraFinal; // Posição final da câmera

    private Rigidbody rb;
    private Animator animator;
    private Vector2 entradaMovimento;
    private bool estaNoChao = true; // Verifica se o jogador está no chão
    private bool cruzouLinhaChegada = false; // Indica se o jogador já cruzou a linha
    private bool movimentoAtivo = false;

    public float pontuacao = 0; // Pontuação do jogador
    private bool caiu = false;
    public float distanciaFrente = 5f;
    private int latasPerdidas = 3;
    private bool imune = false; // Controla a imunidade


    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void Update() {
        if (!movimentoAtivo || cruzouLinhaChegada) return;

        // Verificar se está no chão
        VerificarChao();

        // Atualizar animações
        AtualizarAnimacoes();

        // Reposicionar se cair
        if (transform.position.y < -4 && !caiu) {
            caiu = true;
            Reposicionar();
        }
    }

    void FixedUpdate() {
        if (!movimentoAtivo || cruzouLinhaChegada) return;

        Mover();
    }

    public void OnMove(InputAction.CallbackContext context) {
        entradaMovimento = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed && estaNoChao) {
            Pular();
        }
    }

    public void SetMovimentoAtivo(bool ativo) {
        movimentoAtivo = ativo;
    }

    private void VerificarChao() {
        // Detecta se o jogador está no chão usando Physics.CheckSphere
        estaNoChao = Physics.CheckSphere(detectorChao.position, raioDeteccao, camadaChao);
    }

    private void AtualizarAnimacoes() {
        bool estaMovendo = entradaMovimento.magnitude > 0.1f;
        animator.SetBool("isMoving", estaMovendo);
        animator.SetBool("isGrounded", estaNoChao);
    }

    public void Pular() {
        rb.AddForce(Vector3.up * forcaPulo, ForceMode.Impulse);
        animator.SetBool("isJumping", true); // Ativa a animação de pular
    }

    private void Reposicionar() {
        PerdeLatas(latasPerdidas);
        Transform cameraTransform = Camera.main.transform;

        Vector3 posicaoNaFrente = cameraTransform.position + cameraTransform.forward * distanciaFrente;
        posicaoNaFrente.y = 5f;

        // Reposicionar o personagem em vez de instanciar
        transform.position = posicaoNaFrente;
        transform.rotation = Quaternion.identity;

        caiu = false; // Permite que ele caia novamente no futuro
    }

    public void Mover() {
        Vector3 direcao = new Vector3(entradaMovimento.x, 0, entradaMovimento.y).normalized;

        if (direcao.magnitude >= 0.1f) {
            // Rotação suave
            float anguloAlvo = Mathf.Atan2(direcao.x, direcao.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, anguloAlvo, 0), Time.fixedDeltaTime * 10f);

            // Movimento
            Vector3 movimento = direcao * velocidade * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movimento);
        }
    }

    public void ColetarLatas() {
        pontuacao++;
        textoPonto.text = $"P{playerID} x{pontuacao}";
        // Salva a pontuação sempre que coletar uma lata
        SalvarPontuacao();
    }

    private void SalvarPontuacao() {
        string chavePontuacao = $"PontuacaoJogador{playerID}"; // Gera uma chave única
        PlayerPrefs.SetFloat(chavePontuacao, pontuacao); // Salva a pontuação usando a chave única
        PlayerPrefs.Save(); // Garante que o dado seja salvo imediatamente
        Debug.Log($"Pontuação do Jogador {playerID} salva: {pontuacao}");
    }

    public void PerdeLatas(float latasPerdidas) {
        if (!imune) {
            if (pontuacao < latasPerdidas) {
                pontuacao -= pontuacao;
            } else {
                pontuacao -= latasPerdidas;
            }
            textoPonto.text = $"P{playerID} x{pontuacao}";
            StartCoroutine(Invencibilidade());
            StartCoroutine(DesaparecerTexto());
        }
    }

    private IEnumerator Invencibilidade() {
        imune = true; // Ativa a imunidade
        velocidade = 0; // Para o jogador
        float tempoImune = 1f; // Tempo de invencibilidade (em segundos)
        float intervaloPiscar = 0.2f; // Intervalo entre os piscamentos

        // Efeito de piscamento alternando a visibilidade do modelo
        for (float tempo = 0; tempo < tempoImune; tempo += intervaloPiscar) {
            //animator.SetTrigger("Atingido");
            yield return new WaitForSeconds(intervaloPiscar);
        }

        imune = false;
        velocidade = 5; // Restaura a velocidade do jogador
    }

    public void CruzarLinhaDeChegada() {
        if (!cruzouLinhaChegada) // Garante que o jogador só termine uma vez
        {
            cruzouLinhaChegada = true;
            animator.SetBool("isMoving", true); // Ativa a animação de andar (se necessário)
            StartCoroutine(AndarAtePontoFinal());
        }
    }


    private IEnumerator AndarAtePontoFinal() {
        float velocidadeFinal = 2f; // Velocidade do jogador ao final
        Vector3 direcao = (pontoFinal.position - transform.position).normalized;

        while (Vector3.Distance(transform.position, pontoFinal.position) > 0.1f) {
            transform.position += direcao * velocidadeFinal * Time.deltaTime;
            yield return null;
        }

        // Ajusta a posição exata e orientação
        transform.position = pontoFinal.position;
        transform.LookAt(cameraFinal.position); // Faz o jogador olhar para a câmera
        animator.SetBool("isMoving", false); // Para a animação de andar

        // Notifica o GameManager que este jogador terminou
        GameManagerCaminhoEnergizado gameManager = FindObjectOfType<GameManagerCaminhoEnergizado>();
        if (gameManager != null) {
            gameManager.JogadorTerminou();
        }

        // Exibe mensagem de vitória (opcional)
        Debug.Log("Você terminou a corrida!");
    }

    IEnumerator DesaparecerTexto() {
        textoPerdeuLatas.gameObject.SetActive(true);
        Color corTexto = textoPerdeuLatas.color;
        corTexto.a = 1;
        textoPerdeuLatas.color = corTexto;
        yield return new WaitForSeconds(0.5f);
        float contador = 0;
        while (textoPerdeuLatas.color.a > 0) {
            contador += Time.deltaTime / 0.5f;
            corTexto.a = Mathf.Lerp(1, 0, contador);
            textoPerdeuLatas.color = corTexto;
            if (textoPerdeuLatas.color.a <= 0) {
                textoPerdeuLatas.gameObject.SetActive(false);
            }
            yield return null;
        }
    }

}
