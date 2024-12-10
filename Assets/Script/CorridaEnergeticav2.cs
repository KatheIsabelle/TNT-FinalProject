using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CorridaEnergeticav2 : MonoBehaviour
{
    public PlayerController[] players; // Associe os jogadores no Inspector
    public Transform[] jogadores; // Transforms dos jogadores
    public Transform[] linhas; // Posi��es finais dos jogadores (linhas)
    public Image[] iconesBotoes; // Imagens dos bot�es para cada jogador
    public Sprite[] botoesSprites;

    public Camera mainCamera;
    public Canvas canvas; // Canvas que exibe a pontua��o dos jogadores
    public Canvas rankingCanvas; // Canvas para exibir o ranking dos jogadores
    public TMP_Text[] rankingNomes; // Textos para os nomes dos jogadores
    public TMP_Text[] rankingPontos; // Textos para os pontos dos jogadores

    public float playerMoveSpeed = 5f, cameraFollowSpeed = 5f;
    public float pontosPorUnidadeX = 10f; // Cada ponto vale 10 unidades no eixo X

    private string[] botoesDisponiveis = { "Xis", "Bolinha", "Quadrado", "Triangulo" }; // Nomes das a��es configuradas no InputActions
    private string[] botoesCertos;
    private int[] pontos;

    private bool aguardandoDelay = false, comandosAtivos = true;

    private Dictionary<int, PlayerController> playerIndexMap; // Mapeia �ndices de jogadores para PlayerController

    private void Start()
    {
        if (players == null || players.Length == 0 || jogadores == null || iconesBotoes == null || linhas == null)
        {
            Debug.LogError("Certifique-se de configurar todos os arrays no Inspector!");
            return;
        }

        botoesCertos = new string[jogadores.Length];
        pontos = new int[jogadores.Length];
        playerIndexMap = new Dictionary<int, PlayerController>();

        for (int i = 0; i < jogadores.Length; i++)
        {
            botoesCertos[i] = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
            AtualizaIcone(iconesBotoes[i], botoesCertos[i]);

            if (players[i] != null)
            {
                playerIndexMap[i] = players[i];
            }
        }
    }


    public void OnBotaoPressionado(InputAction.CallbackContext context)
    {
        if (!context.performed || !comandosAtivos) return;

        string botaoPressionado = context.action.name; // Nome da a��o pressionada
        Debug.Log($"Bot�o pressionado: {botaoPressionado}"); // Log para verificar o nome

        // Identificar qual jogador pressionou o bot�o correto
        for (int i = 0; i < players.Length; i++)
        {
            if (botoesCertos[i] == botaoPressionado) // Verifica se o bot�o correto foi pressionado
            {
                Debug.Log($"Jogador {i + 1} pressionou o bot�o correto!");
                AtualizaPontos(i); // Atualiza pontos apenas para o jogador correto
                RandomizaBotaoParaJogador(i); // Randomiza o bot�o ap�s acertar
                return; // Sai do m�todo, evitando a��es de outros jogadores
            }
        }

        // Caso o bot�o pressionado n�o seja o correto de nenhum jogador
        Debug.LogWarning("Bot�o pressionado n�o corresponde ao jogador correto.");
    }


    private void AtualizaPontos(int playerIndex)
    {
        pontos[playerIndex]++;
        RandomizaBotaoParaJogador(playerIndex);
    }

    private void RandomizaBotaoParaJogador(int playerIndex)
    {
        string novoBotao;
        do
        {
            novoBotao = botoesDisponiveis[Random.Range(0, botoesDisponiveis.Length)];
        } while (novoBotao == botoesCertos[playerIndex]);

        botoesCertos[playerIndex] = novoBotao;
        AtualizaIcone(iconesBotoes[playerIndex], novoBotao);
    }

    private void AtualizaIcone(Image icone, string botao)
    {
        int index = System.Array.IndexOf(botoesDisponiveis, botao);
        if (index >= 0 && index < botoesSprites.Length)
        {
            icone.sprite = botoesSprites[index];
        }
    }

    public void OcultarRanking()
    {
        rankingCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!aguardandoDelay && comandosAtivos)
        {
            StartCoroutine(MovimentaJogadores());
        }
    }

    private IEnumerator MovimentaJogadores()
    {
        aguardandoDelay = true;
        yield return new WaitForSeconds(5f); // Tempo de espera antes de iniciar o movimento

        // Desativa o canvas que mostra os bot�es ap�s o delay
        canvas.gameObject.SetActive(false);

        comandosAtivos = false; // Desativa os comandos enquanto os jogadores se movem

        // Atualiza as posi��es de todos os jogadores com base nos pontos
        for (int i = 0; i < jogadores.Length; i++)
        {
            if (pontos[i] > 0)
            {
                // Move a posi��o final do jogador de acordo com os pontos
                linhas[i].position = new Vector3(pontos[i] * pontosPorUnidadeX, linhas[i].position.y, linhas[i].position.z);
            }
        }

        // Calcula o tempo de movimento para cada jogador
        float[] temposTotais = new float[jogadores.Length];
        for (int i = 0; i < jogadores.Length; i++)
        {
            temposTotais[i] = Vector3.Distance(jogadores[i].position, linhas[i].position) / playerMoveSpeed;
        }

        float tempoDecorrido = 0f;
        while (tempoDecorrido < temposTotais.Max()) // Enquanto algum jogador n�o atingir sua posi��o final
        {
            tempoDecorrido += Time.deltaTime;

            // Move todos os jogadores
            for (int i = 0; i < jogadores.Length; i++)
            {
                if (tempoDecorrido < temposTotais[i])
                {
                    jogadores[i].position = Vector3.MoveTowards(jogadores[i].position, linhas[i].position, Time.deltaTime * playerMoveSpeed);
                }
            }

            // Atualiza a posi��o da c�mera para seguir o jogador mais avan�ado
            int jogadorComMaisPontos = System.Array.IndexOf(pontos, pontos.Max());
            mainCamera.transform.position = new Vector3(jogadores[jogadorComMaisPontos].position.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);

            yield return null;
        }

        // Ap�s todos chegarem, a c�mera fica na posi��o final do jogador mais avan�ado
        int finalIndex = System.Array.IndexOf(pontos, pontos.Max());
        mainCamera.transform.position = new Vector3(linhas[finalIndex].position.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);

        ExibirRanking();
    }

    public void ExibirRanking()
    {
        var ranking = pontos.Select((valor, indice) => new { Indice = indice, Valor = valor })
                            .OrderByDescending(p => p.Valor)
                            .ToArray();

        for (int i = 0; i < ranking.Length; i++)
        {
            int jogadorIndex = ranking[i].Indice;
            rankingNomes[i].text = $"Player {jogadorIndex + 1}";
            rankingPontos[i].text = (pontos[jogadorIndex] * 10f).ToString();
        }

        rankingCanvas.gameObject.SetActive(true);
    }
}
