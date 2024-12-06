using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CorridaEnergetica : MonoBehaviour
{
    public PlayerController[] players; // Associe os jogadores no Inspector
    public Transform[] jogadores; // Transforms dos jogadores
    public Transform[] linhas; // Posições finais dos jogadores (linhas)
    public Image[] iconesBotoes; // Imagens dos botões para cada jogador
    public Sprite[] botoesSprites;

    public Camera mainCamera;
    public Canvas canvas; // Canvas que exibe a pontuação dos jogadores
    public Canvas rankingCanvas; // Canvas para exibir o ranking dos jogadores
    public TMP_Text[] rankingNomes; // Textos para os nomes dos jogadores
    public TMP_Text[] rankingPontos; // Textos para os pontos dos jogadores

    public float playerMoveSpeed = 5f, cameraFollowSpeed = 5f;
    public float pontosPorUnidadeX = 10f; // Cada ponto vale 10 unidades no eixo X

    private string[] botoesDisponiveis = { "Xis", "Bolinha", "Quadrado", "Triangulo" }; // Nomes das ações configuradas no InputActions
    private string[] botoesCertos;
    private int[] pontos;

    private bool aguardandoDelay = false, comandosAtivos = true;

    private Dictionary<int, PlayerController> playerIndexMap; // Mapeia índices de jogadores para PlayerController

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

        // Mapeando jogadores com os dispositivos corretos
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

        string botaoPressionado = context.action.name; // Nome da ação pressionada

        for (int i = 0; i < players.Length; i++)
        {
            // Verificando qual jogador pressionou o botão
            if (context.control.device == players[i].GetComponent<PlayerInput>().devices.FirstOrDefault())
            {
                Debug.Log($"Botão pressionado pelo jogador {i + 1}: {botaoPressionado}");

                if (botaoPressionado == botoesCertos[i])
                {
                    AtualizaPontos(i);
                }
                else
                {
                    Debug.Log($"Jogador {i + 1} pressionou o botão errado: {botaoPressionado}");
                }
                return;
            }
        }

        Debug.LogWarning("Dispositivo não associado a um jogador.");
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

        // Desativa o canvas que mostra os botões após o delay
        canvas.gameObject.SetActive(false);

        // Habilita o canvas para mostrar os pontos
        
        comandosAtivos = false; // Desativa os comandos enquanto os jogadores se movem

        // Teleporta as linhas de acordo com os pontos dos jogadores
        for (int i = 0; i < jogadores.Length; i++)
        {
            if (pontos[i] > 0)
            {
                linhas[i].position = new Vector3(pontos[i] * pontosPorUnidadeX, linhas[i].position.y, linhas[i].position.z);
            }
        }

        // Determina o jogador com mais pontos
        int jogadorComMaisPontos = System.Array.IndexOf(pontos, pontos.Max());

        // Calcula a posição final para o jogador com mais pontos
        Vector3 destinoJogador = linhas[jogadorComMaisPontos].position;

        // Agora movemos o jogador até o destino e a câmera o segue
        float tempoTotal = Vector3.Distance(jogadores[jogadorComMaisPontos].position, destinoJogador) / playerMoveSpeed;
        float tempoDecorrido = 0f;

        // Enquanto o jogador estiver se movendo, a câmera deve acompanhá-lo
        while (tempoDecorrido < tempoTotal)
        {
            tempoDecorrido += Time.deltaTime;

            // Move o jogador até o destino
            jogadores[jogadorComMaisPontos].position = Vector3.MoveTowards(jogadores[jogadorComMaisPontos].position, destinoJogador, Time.deltaTime * playerMoveSpeed);

            // A câmera segue a linha do jogador com mais pontos, durante o movimento
            mainCamera.transform.position = new Vector3(jogadores[jogadorComMaisPontos].position.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);

            yield return null; // Espera até o próximo frame
        }

        // Garantir que a câmera chegue exatamente na posição final do jogador
        mainCamera.transform.position = new Vector3(destinoJogador.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);

        // Exibe o ranking após a movimentação
        ExibirRanking();
    }

    // Função para exibir o ranking
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

        // Agora ativa o canvas de ranking
        rankingCanvas.gameObject.SetActive(true);
    }



}
