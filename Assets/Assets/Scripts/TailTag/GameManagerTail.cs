using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necess�rio para TextMeshPro

public class GameManagerTail : MonoBehaviour
{
    public GameObject tailPrefab;

    public float gameDuration = 60f;
    public TMP_Text timerText; // Texto para o cron�metro
    public TMP_Text winnerText; // Texto para o vencedor
    public TMP_Text rankingText; // Texto para exibir o ranking

    private float remainingTime;

    private void Start()
    {
        remainingTime = gameDuration;


        // Instanciar a cauda no centro
        Instantiate(tailPrefab, new Vector3(0f, 3f, 22), Quaternion.Euler(-90, 200, 0));

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

        // Adiciona pontos finais para os jogadores que est�o com a cauda
        foreach (var player in players)
        {
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            controller.AddEndGamePoints();
        }

        // Ordenar os jogadores pelo n�mero de pontos
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