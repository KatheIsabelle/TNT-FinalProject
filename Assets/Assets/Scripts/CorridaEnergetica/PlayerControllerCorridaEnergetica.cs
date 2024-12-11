using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerControllerCorridaEnergetica : MonoBehaviour
{
    public Image[] iconesBotoes;  // Array de �cones para os bot�es
    public Sprite[] botoesSprites;
    private string botaoCerto;
    public int pontos;
    string[] botoesDisponiveis = { "Jump", "Dash", "Attack", "Extra" }; // Nomes das a��es configuradas no InputActions

    private bool SquarePressed = false;
    private bool xPressed = false;
    private bool TrianglePressed = false;
    private bool CirclePressed = false;
    private bool comandosAtivos = true;  // Vari�vel para controlar comandos

    public CorridaEnergetica corrida; // Refer�ncia para o script CorridaEnergetica
    public int playerIndex; // �ndice do jogador na corrida
    private PlayerInput playerInput; // Refer�ncia para o PlayerInput espec�fico do jogador

    private Vector3 lastPosition;
    [SerializeField] private Transform myTransform;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
   
        playerInput = GetComponent<PlayerInput>(); // Obt�m o PlayerInput do jogador
        animator = GetComponent<Animator>();
        for (int i = 0; i < iconesBotoes.Length; i++)  // Atualiza para todos os jogadores
        {
            string botoesCertos = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
            AtualizaIcone(iconesBotoes[i], botoesCertos);  // Atualiza o �cone de acordo com o jogador
        }
        RandomizaBotaoParaJogador();  // Garante que o bot�o correto seja escolhido para o jogador
    }

    private void AtualizaPontos()
    {
        pontos += 10; // Adiciona 10 pontos
        if (corrida != null)
        {
            corrida.AtualizarPontos(playerIndex, 10); // Atualiza os pontos no script CorridaEnergetica
        }
        else
        {
            Debug.LogWarning("Refer�ncia para CorridaEnergetica n�o foi atribu�da.");
        }
        RandomizaBotaoParaJogador(); // Randomiza o bot�o ap�s acertar
    }


    private void RemovePontos()
    {
        if (pontos > 0) // Garante que os pontos n�o fiquem negativos
        {
            pontos -= 10; // Diminui 10 pontos
            if (corrida != null)
            {
                corrida.AtualizarPontos(playerIndex, -10); // Atualiza os pontos no script CorridaEnergetica
            }
            else
            {
                Debug.LogWarning("Refer�ncia para CorridaEnergetica n�o foi atribu�da.");
            }
        }
        else
        {
            Debug.Log($"Jogador {playerIndex} j� est� com 0 pontos. N�o � poss�vel decrementar.");
        }
    }


    private void RandomizaBotaoParaJogador()
    {
        string novoBotao;
        do
        {
            novoBotao = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
        } while (novoBotao == botaoCerto);  // Garante que o novo bot�o seja diferente do anterior

        botaoCerto = novoBotao;

        // Atualiza o �cone para todos os jogadores
        for (int i = 0; i < iconesBotoes.Length; i++)  // Agora atualiza para todos os jogadores
        {
            AtualizaIcone(iconesBotoes[i], novoBotao);  // Atualiza o �cone de cada jogador
        }
    }

    private void AtualizaIcone(Image icone, string botao)
    {
        int index = System.Array.IndexOf(botoesDisponiveis, botao);
        if (index >= 0 && index < botoesSprites.Length)
        {
            icone.sprite = botoesSprites[index];
        }
        else
        {
            Debug.LogError("�ndice fora dos limites! O bot�o " + botao + " n�o tem um sprite correspondente.");
        }
    }

    // O m�todo de Input deve ser mapeado para um bot�o espec�fico por jogador.
    public void OnBotaoPressionado(InputAction.CallbackContext context)
    {
        if (!context.performed || !comandosAtivos) return;

        string botaoPressionado = context.action.name; // Nome da a��o pressionada

        if (string.Equals(botaoCerto, botaoPressionado, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Jogador {playerIndex} pressionou o bot�o correto: {botaoPressionado}.");
            AtualizaPontos(); // Adiciona pontos
        }
        else
        {
            Debug.Log($"Jogador {playerIndex} pressionou o bot�o errado: {botaoPressionado} (esperado: {botaoCerto}).");
            RemovePontos(); // Remove pontos
        }
    }


    public void OnSquarePressed(InputAction.CallbackContext context)
    {
        SquarePressed = context.action.triggered;
    }

    public void OnXPressed(InputAction.CallbackContext context)
    {
        xPressed = context.action.triggered;
    }

    public void OnTrianglePressed(InputAction.CallbackContext context)
    {
        TrianglePressed = context.action.triggered;
    }

    public void OnCirclePressed(InputAction.CallbackContext context)
    {
        CirclePressed = context.action.triggered;
    }

    void Update()
    {
        if (myTransform.position != lastPosition) {
            
            animator.SetBool("isMoving", true);

        } else {
            
            animator.SetBool("isMoving", false);
        }
        
        lastPosition = myTransform.position;

        // Debugando os bot�es pressionados, se necess�rio
        //if (SquarePressed) Debug.Log(gameObject.name + " apertou o bot�o Quadrado");
        //if (xPressed) Debug.Log(gameObject.name + " apertou o bot�o X");
        //if (CirclePressed) Debug.Log(gameObject.name + " apertou o bot�o Bolinha");
        //if (TrianglePressed) Debug.Log(gameObject.name + " apertou o bot�o Triangulo");
    }
}
