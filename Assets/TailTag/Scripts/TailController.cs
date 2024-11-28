using UnityEngine;

public class TailController : MonoBehaviour
{
    private bool isPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            PlayerControllerTail player = other.GetComponent<PlayerControllerTail>();
            if (player != null)
            {
                isPickedUp = true;

                // Marca o jogador como possuidor da cauda
                player.hasTail = true;

                // Move a cauda para o jogador
                transform.parent = player.transform;
                transform.localPosition = new Vector3(0, 0.5f, -0.5f);
            }
        }
    }
}
