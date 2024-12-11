using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Necessário para TextMeshPro

public class GameManagerTail : MonoBehaviour
{
    public GameObject tailPrefab;

    public float gameDuration = 60f;
    public TMP_Text timerText; // Texto para o cronômetro
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
        GameData.playerRankings.Clear();
        foreach (var player in players) {
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            GameData.playerRankings.Add(new PlayerData(player.name, controller.points, player));
        }

        // Salva a cena do jogo e carrega a cena do pódio
        SceneManager.LoadScene("PodiumScene");

        // Após carregar a cena do pódio, garanta que os jogadores tenham a escala certa e estão desabilitados
        StartCoroutine(EnsureCorrectScaleAndDisableMovement());
    }
    // Corrige a escala dos jogadores após a cena do pódio ser carregada
    private IEnumerator EnsureCorrectScaleAndDisableMovement() {
        // Aguarde o final da transição de cena
        yield return new WaitForSeconds(0.5f);

        // Encontre todos os jogadores na nova cena
        GameObject[] playersInPodium = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in playersInPodium) {
            // Resetando a escala dos jogadores para (1, 1, 1)
            player.transform.localScale = Vector3.one;

            // Desabilitar o controle do jogador
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            if (controller != null) {
                controller.enabled = false; // Desabilita o script que controla o movimento do jogador
            }

            // Se o jogador tiver um Rigidbody, desabilite a movimentação também
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.velocity = Vector3.zero; // Parar qualquer movimento residual
                rb.isKinematic = true; // Desabilitar interações físicas
            }
        }
    }
}