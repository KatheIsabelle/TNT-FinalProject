using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GameControllerCirculodeEnergia : MonoBehaviour
{
    #region INSTÂNCIAS
    public static GameControllerCirculodeEnergia Instance;  // Instância única do GameController
    #endregion

    #region SPAWN CANS
    public GameObject[] cansPrefab;  // Array de Prefabs para as latinhas
    public Transform[] spawnPoints;  // Array de pontos de spawn para as latinhas
    public int maxCans = 12;  // Número máximo de latinhas na cena
    private int lastSpawnIndex = -1;  // Índice do último spawn para garantir que não se repitam
    private int currentCansCount = 0;  // Contador de latinhas no jogo
    #endregion

    #region SCORE
    public List<TextMeshProUGUI> playerScoreTexts;  // Lista para armazenar os TMPs dos jogadores
    #endregion

    #region PLATFORMS
    public Transform transformPlat1;  // Referência para a primeira plataforma
    public Transform transformPlat2;  // Referência para a segunda plataforma
    public float rotationSpeed;  // Velocidade de rotação das plataformas
    #endregion

    #region LISTA DE JOGADORES
    private List<PlayerData> players = new List<PlayerData>();  // Lista para armazenar os dados dos jogadores
    #endregion

    // Variável para o contador de IDs dos jogadores
    private int nextPlayerID = 1;  // ID do próximo jogador

    // Variáveis do temporizador
    public float gameDuration = 120f;  // Tempo total do jogo em segundos (2 minutos)
    private float timeRemaining;  // Tempo restante no jogo
    private bool gameOver = false;  // Flag para indicar se o jogo acabou
    public TextMeshProUGUI timeText;  // Referência ao TextMeshPro para exibir o tempo restante

    #region CLASSE PLAYERDATA
    public class PlayerData
    {
        public int playerID;  // ID do jogador
        public int qtdCans;   // Pontuação (quantidade de latinhas coletadas)
        public TextMeshProUGUI playerScoreText;  // Referência ao TMP de pontuação

        public PlayerData(int id)
        {
            playerID = id;
            qtdCans = 0;
        }
    }
    #endregion

    #region MÉTODOS DO GAME CONTROLLER

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeRemaining = gameDuration;
        InvokeRepeating("TrySpawnCan", 1f, 1f);  // Inicia a geração de latinhas
    }

    void Update()
    {
        if (!gameOver)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeText.fontSize = 0;
                EndGame();
            }

            // Converte o tempo restante para minutos e segundos
            int minutes = Mathf.FloorToInt(timeRemaining / 60);  // Obtém os minutos
            int seconds = Mathf.FloorToInt(timeRemaining % 60);  // Obtém os segundos restantes

            // Atualiza o texto do tempo na UI com o formato "MM:SS"
            timeText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);  // Formata com 2 dígitos

            Platforms();
        }
    }

    void TrySpawnCan()
    {
        SpawnCan();  // Gera uma nova latinha
    }

    void SpawnCan()
    {
        int canIndex = Random.Range(0, cansPrefab.Length);
        int spawnIndex;
        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        } while (spawnIndex == lastSpawnIndex);

        // Instancia a latinha no ponto de spawn escolhido
        Instantiate(cansPrefab[canIndex], spawnPoints[spawnIndex].position, Quaternion.identity);
        lastSpawnIndex = spawnIndex;
        currentCansCount++;  // Incrementa o contador de latinhas no jogo
    }

    public void OnCanDestroyed()
    {
        currentCansCount--;  // Decrementa o contador quando uma latinha é destruída
    }

    void Platforms()
    {
        transformPlat1.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
        transformPlat2.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public int RegisterPlayer(TextMeshProUGUI playerScoreText)
    {
        if (players.Count < 4)  // Verifica se há espaço para mais jogadores
        {
            int playerID = nextPlayerID++;  // Atribui um ID incremental
            players.Add(new PlayerData(playerID));  // Adiciona o jogador à lista

            PlayerData player = players.Find(p => p.playerID == playerID);
            if (player != null)
            {
                player.playerScoreText = playerScoreText;  // Atribui o TMP de pontuação
            }

            Debug.Log($"Player registrado com ID: {playerID}");
            return playerID;  // Retorna o ID do jogador
        }
        else
        {
            return -1;  // Retorna -1 caso não seja possível registrar mais jogadores
        }
    }

    public void AddScore(int playerID, int points)
    {
        PlayerData player = players.Find(p => p.playerID == playerID);
        if (player != null)
        {
            player.qtdCans += points;

            int playerIndex = players.IndexOf(player);
            if (playerIndex >= 0 && playerIndex < playerScoreTexts.Count)
            {
                playerScoreTexts[playerIndex].text = "" + player.qtdCans.ToString();
            }
        }
        else
        {
            Debug.LogError($"Player com ID {playerID} não encontrado!");
        }
    }

    List<PlayerData> GetTopPlayers()
    {
        // Ordena os jogadores pela pontuação, do maior para o menor
        List<PlayerData> sortedPlayers = new List<PlayerData>(players);
        sortedPlayers.Sort((player1, player2) => player2.qtdCans.CompareTo(player1.qtdCans)); // Ordenação decrescente

        return sortedPlayers;
    }


    void EndGame()
    {
        gameOver = true;

        // Obter os 4 primeiros jogadores
        List<PlayerData> topPlayers = GetTopPlayers();

        // Exibir os 4 primeiros lugares
        for (int i = 0; i < topPlayers.Count && i < 4; i++)
        {
            PlayerData player = topPlayers[i];
            Debug.Log($"Posição {i + 1}: Jogador {player.playerID} com {player.qtdCans} pontos");
        }

        // Caso não tenha 4 jogadores, você pode exibir as posições restantes como "Empate" ou "Sem jogador"
        for (int i = topPlayers.Count; i < 4; i++)
        {
            Debug.Log($"Posição {i + 1}: Sem jogador");
        }

        // Pausar o jogo após a exibição do pódio
        Time.timeScale = 0;
    }


    #endregion
}
