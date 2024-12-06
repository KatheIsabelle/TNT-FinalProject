using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
    [Header("Cans Points")]
    public int redCan = 10;
    public int greenCan = 20;
    public int purpleCan = 30;
    public int powerDown = -10;

    [Header("Respawn Cans Settings")]
    public float respawnTime = 3f;
    public Transform[] respawnPoints; 
    public GameObject[] canPrefabs;   

    private Renderer canRenderer;
    private Collider canCollider;
    private bool isDisabled;

    void Start()
    {
        canRenderer = GetComponent<Renderer>();
        canCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDisabled && other.CompareTag("Player"))
        {
            Debug.Log("Player coletou a lata");

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddPoints(GetCanPoints());
            }

            //respawn
            StartCoroutine(RespawnCan());
            Destroy(gameObject, 1f); // Destroi a lata coletada
        }
    }

    private int GetCanPoints()
    {
        if (CompareTag("redCan")) return redCan;
        if (CompareTag("greenCan")) return greenCan;
        if (CompareTag("purpleCan")) return purpleCan;
        if (CompareTag("powerDown")) return powerDown;

        return 0;
    }

    private IEnumerator RespawnCan()
    {
        yield return new WaitForSeconds(respawnTime);

        // Respawn a lata na posição correta
        RespawnAtRandomPoint();
    }

    private void RespawnAtRandomPoint()
    {
        
        if (respawnPoints.Length > 0 && canPrefabs.Length > 0)
        {
            // Seleciona um ponto de respawn aleatório
            Transform randomPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];

            // Seleciona um prefab de lata aleatório
            GameObject randomPrefab = canPrefabs[Random.Range(0, canPrefabs.Length)];

            // Instancia o prefab na posição do ponto de respawn
            GameObject respawnedCan = Instantiate(randomPrefab, randomPoint.position, Quaternion.identity);

            // Faz a lata ficar de frente para a câmera
            Quaternion targetRotation = Quaternion.Euler(0, 90, 0); 
           // respawnedCan.transform.rotation = Quaternion.RotateTowards(respawnedCan.transform.rotation, targetRotation, 90 * Time.deltaTime);

        }
    }
}
