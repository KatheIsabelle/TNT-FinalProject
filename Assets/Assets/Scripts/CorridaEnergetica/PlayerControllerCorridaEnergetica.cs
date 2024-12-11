using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerControllerCorridaEnergetica : MonoBehaviour
{
    public Image[] iconesBotoes;  // Array de ícones para os botões
    public Sprite[] botoesSprites;
    private string botaoCerto;
    public int pontos;
    string[] botoesDisponiveis = { "Jump", "Dash", "Attack", "Extra" }; // Nomes das ações configuradas no InputActions

    private bool SquarePressed = false;
    private bool xPressed = false;
    private bool TrianglePressed = false;
    private bool CirclePressed = false;
    private bool comandosAtivos = true;  // Variável para controlar comandos

    public CorridaEnergetica corrida; // Referência para o script CorridaEnergetica
    public int playerIndex; // Índice do jogador na corrida
    private PlayerInput playerInput; // Referência para o PlayerInput específico do jogador

    private Vector3 lastPosition;
    [SerializeField] private Transform myTransform;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
   
        playerInput = GetComponent<PlayerInput>(); // Obtém o PlayerInput do jogador
        animator = GetComponent<Animator>();
        for (int i = 0; i < iconesBotoes.Length; i++)  // Atualiza para todos os jogadores
        {
            string botoesCertos = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
            AtualizaIcone(iconesBotoes[i], botoesCertos);  // Atualiza o ícone de acordo com o jogador
        }
        RandomizaBotaoParaJogador();  // Garante que o botão correto seja escolhido para o jogador
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
            Debug.LogWarning("Referência para CorridaEnergetica não foi atribuída.");
        }
        RandomizaBotaoParaJogador(); // Randomiza o botão após acertar
    }


    private void RemovePontos()
    {
        if (pontos > 0) // Garante que os pontos não fiquem negativos
        {
            pontos -= 10; // Diminui 10 pontos
            if (corrida != null)
            {
                corrida.AtualizarPontos(playerIndex, -10); // Atualiza os pontos no script CorridaEnergetica
            }
            else
            {
                Debug.LogWarning("Referência para CorridaEnergetica não foi atribuída.");
            }
        }
        else
        {
            Debug.Log($"Jogador {playerIndex} já está com 0 pontos. Não é possível decrementar.");
        }
    }


    private void RandomizaBotaoParaJogador()
    {
        string novoBotao;
        do
        {
            novoBotao = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
        } while (novoBotao == botaoCerto);  // Garante que o novo botão seja diferente do anterior

        botaoCerto = novoBotao;

        // Atualiza o ícone para todos os jogadores
        for (int i = 0; i < iconesBotoes.Length; i++)  // Agora atualiza para todos os jogadores
        {
            AtualizaIcone(iconesBotoes[i], novoBotao);  // Atualiza o ícone de cada jogador
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
            Debug.LogError("Índice fora dos limites! O botão " + botao + " não tem um sprite correspondente.");
        }
    }

    // O método de Input deve ser mapeado para um botão específico por jogador.
    public void OnBotaoPressionado(InputAction.CallbackContext context)
    {
        if (!context.performed || !comandosAtivos) return;

        string botaoPressionado = context.action.name; // Nome da ação pressionada

        if (string.Equals(botaoCerto, botaoPressionado, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Jogador {playerIndex} pressionou o botão correto: {botaoPressionado}.");
            AtualizaPontos(); // Adiciona pontos
        }
        else
        {
            Debug.Log($"Jogador {playerIndex} pressionou o botão errado: {botaoPressionado} (esperado: {botaoCerto}).");
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

        // Debugando os botões pressionados, se necessário
        //if (SquarePressed) Debug.Log(gameObject.name + " apertou o botão Quadrado");
        //if (xPressed) Debug.Log(gameObject.name + " apertou o botão X");
        //if (CirclePressed) Debug.Log(gameObject.name + " apertou o botão Bolinha");
        //if (TrianglePressed) Debug.Log(gameObject.name + " apertou o botão Triangulo");
    }
}
