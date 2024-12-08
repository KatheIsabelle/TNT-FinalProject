using UnityEngine;
using System.Collections;
using TMPro;

public class GameController : MonoBehaviour
{
    //INSTÂNCIAS
    private PlayerController currentPlayer; // Referência ao PlayerController atual
    public static GameController Instance;

    
    //SPAWN CANS
    [Tooltip("Prefab da Latinha")]
    public GameObject[] cansPrefab;

    [Tooltip("Pontos de spawn para as Latinhas")]
    public Transform[] spawnPoints;

    [Tooltip("Total máximo de latinhas que podem existir na cena")]
    public int maxCans = 12;

    // Armazena o índice do último spawn
    private int lastSpawnIndex = -1; 

    // Contador de latinhas atuais na cena
    private int currentCansCount = 0;  


    // SCORE
    public float qtdCans;
    public TextMeshProUGUI scoreText; // Referência ao TMP na UI

    //PLATFORMS
    public Transform transformPlat1;
    public Transform transformPlat2;
    public float rotationSpeed;

    void Awake()
    {
        // Instância do GameController
        Instance = this;
    }

    void Start()
    {
        // Inicializa a Instância do Player
        currentPlayer = PlayerController.Instance;

        // Começa a gerar Latinhas a cada 1 segundo
        InvokeRepeating("TrySpawnCan", 1f, 1f);  // Começa 1 segundo após o início, e repete a cada 1 segundo
    }

    void Update()
    {
        Scores();
        Platforms();
    }

    void TrySpawnCan()
    {
        // Verifica se já temos o número máximo de Latinhas na cena
        if (currentCansCount < maxCans)
        {
            SpawnCan();
        }
    }

    void SpawnCan()
    {
        // Escolhe aleatoriamente um prefab de latinha a partir do array cansPrefabs
        int canIndex = Random.Range(0, cansPrefab.Length);

        // Escolhe um ponto de spawn diferente do último
        int spawnIndex;
        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        } while (spawnIndex == lastSpawnIndex); // Garante que seja diferente do último ponto

        // Instancia a latinha no ponto escolhido
        Instantiate(cansPrefab[canIndex], spawnPoints[spawnIndex].position, Quaternion.identity);

        // Atualiza o índice do último spawn
        lastSpawnIndex = spawnIndex;

        // Atualiza o contador de latinhas
        currentCansCount++;
    }

    // Função para ser chamada quando uma latinha é estourada
    public void OnCanDestroyed()
    {
        // Reduz o contador de latinhas quando uma latinha é destruída
        currentCansCount--;
    }

    void Scores()
    {
        //Debug.Log(qtdCans);
        scoreText.text = "" + qtdCans;
    }

    void Platforms()
    {
        //Platform 1

        transformPlat1.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);

        //Platform 2

        transformPlat2.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
