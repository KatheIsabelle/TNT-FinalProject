using UnityEngine;

public class CansManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform[] respawnPoints; 
    public GameObject[] canPrefabs;   
    public int initialCans = 1;       

    void Start()
    {
        SpawnInitialCans();
    }

    private void SpawnInitialCans()
    {
        if (respawnPoints.Length > 0 && canPrefabs.Length > 0)
        {
            for (int i = 0; i < initialCans; i++)
            {
                // Seleciona um ponto de respawn aleatório
                Transform randomPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];

                // Seleciona um prefab de lata aleatório
                GameObject randomPrefab = canPrefabs[Random.Range(0, canPrefabs.Length)];

                // Instancia o prefab na posição do ponto de respawn
                GameObject spawnedCan = Instantiate(randomPrefab, randomPoint.position, Quaternion.identity);

                // Passa os pontos de respawn e os prefabs para o Collectable
                Collectable collectableScript = spawnedCan.GetComponent<Collectable>();
                if (collectableScript != null)
                {
                    collectableScript.respawnPoints = respawnPoints;
                    collectableScript.canPrefabs = canPrefabs;
                }
            }
        }
    }
}
