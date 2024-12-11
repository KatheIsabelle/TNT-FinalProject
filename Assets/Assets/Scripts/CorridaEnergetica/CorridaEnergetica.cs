using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CorridaEnergetica : MonoBehaviour
{
    public PlayerControllerCorridaEnergetica[] players; // Associe os jogadores no Inspector
    public Transform[] jogadores; // Transforms dos jogadores
    public Transform[] linhas; // Posições finais dos jogadores (linhas)

    public Camera mainCamera;
    public Canvas canvas; // Canvas que exibe a pontuação dos jogadores
    public Canvas rankingCanvas; // Canvas para exibir o ranking dos jogadores
    public TMP_Text[] rankingNomes; // Textos para os nomes dos jogadores
    public TMP_Text[] rankingPontos; // Textos para os pontos dos jogadores

    public float playerMoveSpeed = 10f, cameraFollowSpeed = 10f;
    public float pontosPorUnidadeX = 1f; // Cada ponto vale 1 unidade no eixo X

    private int[] pontos; // Armazena os pontos de cada jogador
    private bool aguardandoDelay = false, comandosAtivos = true;
    private bool corridaIniciada = false; // Garantir que a corrotina não seja chamada múltiplas vezes

    public float gameDuration = 30f;
    public TMP_Text timerText; // Texto para o cronômetro
    private float remainingTime;

    private void Start()
    {
        remainingTime = gameDuration;
        pontos = new int[players.Length]; // Inicializa o array de pontos para cada jogador
        rankingCanvas.gameObject.SetActive(false); // Certifique-se de que o rankingCanvas está desativado inicialmente

    }

    public void OcultarRanking()
    {
        rankingCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);
        if (remainingTime <= 0) {
            timerText.fontSize = 0f;
        }
        if (!aguardandoDelay && comandosAtivos && !corridaIniciada)
        {
            corridaIniciada = true;  // Marca que a corrida começou
            StartCoroutine(MovimentaJogadores());
        }
    }

    private IEnumerator MovimentaJogadores()
    {
        Debug.Log("MovimentaJogadores iniciado");

        aguardandoDelay = true;
        yield return new WaitForSeconds(30f); // Tempo de espera antes de iniciar o movimento

        // Desativa o canvas que mostra os botões após o delay
        canvas.gameObject.SetActive(false);

        comandosAtivos = false; // Desativa os comandos enquanto os jogadores se movem

        // Atualiza as posições de todos os jogadores com base nos pontos
        for (int i = 0; i < jogadores.Length; i++)
        {
            if (pontos[i] > 0)
            {
                // Move a posição final do jogador de acordo com os pontos
                linhas[i].position = new Vector3(pontos[i] * pontosPorUnidadeX, linhas[i].position.y, linhas[i].position.z);
            }
        }

        // Calcula o tempo de movimento para cada jogador
        float[] temposTotais = new float[jogadores.Length];
        for (int i = 0; i < jogadores.Length; i++)
        {
            temposTotais[i] = Vector3.Distance(jogadores[i].position, linhas[i].position) / playerMoveSpeed;
            Debug.Log("Tempo Total para jogador " + i + ": " + temposTotais[i]);
        }

        float tempoDecorrido = 0f;
        bool[] jogadoresChegaram = new bool[jogadores.Length];

        while (tempoDecorrido < temposTotais.Max()) // Enquanto algum jogador não atingir sua posição final
        {
            tempoDecorrido += Time.deltaTime;

            // Move todos os jogadores
            for (int i = 0; i < jogadores.Length; i++)
            {
                if (!jogadoresChegaram[i] && tempoDecorrido < temposTotais[i])
                {
                    jogadores[i].position = Vector3.MoveTowards(jogadores[i].position, linhas[i].position, Time.deltaTime * playerMoveSpeed);
                }

                // Marca o jogador como "chegado" quando atingir sua posição final
                if (jogadores[i].position == linhas[i].position)
                {
                    jogadoresChegaram[i] = true;
                }
            }

            // Atualiza a posição da câmera para seguir o jogador mais avançado
            int jogadorComMaisPontos = System.Array.IndexOf(pontos, pontos.Max());
            mainCamera.transform.position = new Vector3(jogadores[jogadorComMaisPontos].position.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);

            yield return null;
        }

        // Após todos chegarem, a câmera fica na posição final do jogador mais avançado
        int finalIndex = System.Array.IndexOf(pontos, pontos.Max());
        mainCamera.transform.position = new Vector3(linhas[finalIndex].position.x + 5f, mainCamera.transform.position.y, mainCamera.transform.position.z);

        Debug.Log("Chamada ExibirRanking");
        ExibirRanking(); // Confirme se o método é chamado aqui
    }

    public void ExibirRanking()
    {
        // Ordena os jogadores de acordo com seus pontos
        var ranking = pontos.Select((valor, indice) => new { Indice = indice, Valor = valor })
                            .OrderByDescending(p => p.Valor)
                            .ToArray();

        // Atualiza o ranking com os nomes e pontos
        for (int i = 0; i < ranking.Length; i++)
        {
            int jogadorIndex = ranking[i].Indice;
            rankingNomes[i].text = $"Player {jogadorIndex + 1}";  // Atualiza o nome do jogador
            rankingPontos[i].text = (pontos[jogadorIndex] * 1f).ToString();  // Atualiza os pontos com a conversão para o valor adequado
        }

        // Exibe o canvas de ranking
        rankingCanvas.gameObject.SetActive(true);

        // Debug para verificar se o canvas está sendo ativado
        Debug.Log("Ranking Canvas ativado");
    }

    // Função chamada pelos scripts de cada jogador para atualizar os pontos
    public void AtualizarPontos(int jogadorIndex, int pontosGanho)
    {
        pontos[jogadorIndex] += pontosGanho;  // Atualiza os pontos do jogador
    }
}
