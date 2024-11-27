using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public static int coinCount = 0; // Contador global de moedas

    void OnTriggerEnter(Collider other)
    {
        //colisão com player
        if (other.CompareTag("Player"))
        {
            coinCount++; // Incrementa o contador
            Debug.Log("Moedas coletadas: " + coinCount);
            Destroy(gameObject); 
        }
    }
}
