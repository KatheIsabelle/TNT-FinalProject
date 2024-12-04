using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necessário para TextMeshPro

public class GameManagerTail : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tailPrefab;
    public Transform[] spawnPoints;

    public GameObject[] dynamicObstaclePrefabs; // Array de obstáculos dinâmicos
    public Transform[] obstacleSpawnPoints;    // Pontos de spawn dos obstáculos dinâmicos

    public float gameDuration = 60f;
    public TMP_Text timerText; // Texto para o cronômetro
    public TMP_Text winnerText; // Texto para o vencedor
    public TMP_Text rankingText; // Texto para exibir o ranking

    private float remainingTime;

    private void Start()
    {
        remainingTime = gameDuration;

        // Instanciar os jogadores
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);
            player.name = "Player " + (i + 1);

            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            controller.playerId = i + 1;
        }

        // Instanciar a cauda no centro
        Instantiate(tailPrefab, new Vector3(-0.7f, 0.7f, 50), Quaternion.Euler(0, -230, 0));

        // Instanciar obstáculos dinâmicos
        foreach (Transform spawnPoint in obstacleSpawnPoints)
        {
            GameObject obstaclePrefab = dynamicObstaclePrefabs[Random.Range(0, dynamicObstaclePrefabs.Length)];
            Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);

        if (remainingTime <= 0)
        {
            EndGame();
        }
        else
        {
            UpdateRanking();
        }
    }

    private void UpdateRanking()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        System.Array.Sort(players, (player1, player2) =>
        {
            int points1 = player1.GetComponent<PlayerControllerTail>().points;
            int points2 = player2.GetComponent<PlayerControllerTail>().points;
            return points2.CompareTo(points1); // Ordena de forma decrescente
        });

        string ranking = "Ranking:\n";
        foreach (var player in players)
        {
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            ranking += $"{player.name}: {controller.points} pontos\n";
        }
        rankingText.text = ranking;
    }

    private void EndGame()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Adiciona pontos finais para os jogadores que estão com a cauda
        foreach (var player in players)
        {
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            controller.AddEndGamePoints();
        }

        // Ordenar os jogadores pelo número de pontos
        System.Array.Sort(players, (player1, player2) =>
        {
            int points1 = player1.GetComponent<PlayerControllerTail>().points;
            int points2 = player2.GetComponent<PlayerControllerTail>().points;
            return points2.CompareTo(points1); // Ordena de forma decrescente
        });

        // Exibir vencedor
        PlayerControllerTail winnerController = players[0].GetComponent<PlayerControllerTail>();
        winnerText.text = $"Winner: {players[0].name} with {winnerController.points} points";

        // Atualizar o ranking com todos os jogadores
        string ranking = "Final Ranking:\n";
        for (int i = 0; i < players.Length; i++)
        {
            PlayerControllerTail controller = players[i].GetComponent<PlayerControllerTail>();
            ranking += $"{players[i].name}: {controller.points} pontos\n";
        }
        rankingText.text = ranking; // Atualiza a UI com o ranking final

        Time.timeScale = 0; // Pausa o jogo no final
    }
}
