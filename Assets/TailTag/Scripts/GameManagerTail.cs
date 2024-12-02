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


    private float remainingTime;

    void Start()
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

    void Update()
    {
        remainingTime -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);

        if (remainingTime <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject winner = null;

        foreach (var player in players)
        {
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            if (controller.hasTail)
            {
                winner = player;
                break;
            }
        }

        winnerText.text = winner != null ? "Winner: " + winner.name : "No Winner!";
        Time.timeScale = 0;
    }
}
