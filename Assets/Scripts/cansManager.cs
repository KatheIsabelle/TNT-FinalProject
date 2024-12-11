using UnityEngine;
using System.Collections;

public class CansManager : MonoBehaviour
{
    [Header("Respawn Cans Settings")]
    public float respawnTime = 3f;
    public Transform[] respawnPoints; 
    public GameObject[] canPrefabs;   
    public int initialCans = 1;

    private void Start()
    {
        // Instancia as latas iniciais
        for (int i = 0; i < initialCans; i++)
        {
            SpawnNewCan();
        }
    }

    public void SpawnNewCan()
    {
        StartCoroutine(RespawnCan());
    }

    private IEnumerator RespawnCan()
    {
        yield return new WaitForSeconds(respawnTime);

        if (respawnPoints.Length > 0 && canPrefabs.Length > 0)
        {
            // Seleciona um ponto de respawn aleatório
            Transform randomPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];

            // Seleciona um prefab de lata aleatório
            GameObject randomPrefab = canPrefabs[Random.Range(0, canPrefabs.Length)];

             // Define a rotação desejada
            Quaternion targetRotation = Quaternion.Euler(0, 135, 0);

             // Instancia o prefab na posição e rotação 90graus
            Instantiate(randomPrefab, randomPoint.position, targetRotation);
        }
    }
}
