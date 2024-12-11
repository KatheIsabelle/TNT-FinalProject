using UnityEngine;
using System.Collections.Generic;

public class Collectable : MonoBehaviour
{
    [Header("Cans Points")]
    public int redCan = 10;
    public int greenCan = 20;
    public int purpleCan = 30;
    public int powerDown = -10;


    // Lista de tags com players
    public List<string> validTags = new List<string> { "Player", "Player2", "Player3", "Player4" };

    private void OnTriggerEnter(Collider other)
    {
        if (validTags.Contains(other.gameObject.tag))
        {
            Debug.Log($"{other.gameObject.tag} coletou a lata");

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                int points = GetCanPoints();
                if (points < 0)
                {
                    gameManager.LosePoints(other.gameObject.tag, Mathf.Abs(points));
                }
                else
                {
                    gameManager.AddPoints(other.gameObject.tag, points);
                }
            }

            // Notifica o CansManager para realizar o respawn
            CansManager cansManager = FindObjectOfType<CansManager>();
            if (cansManager != null)
            {
                cansManager.SpawnNewCan();
            }

            Destroy(gameObject); 
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
}
