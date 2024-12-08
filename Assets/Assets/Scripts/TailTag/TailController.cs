using UnityEngine;

public class TailController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que colidiu com a cauda � um jogador
        if (other.CompareTag("Player"))
        {
            // Obt�m o script do jogador que colidiu com a cauda
            PlayerControllerTail newPlayer = other.GetComponent<PlayerControllerTail>();

            // Verifica se o jogador n�o tem cauda e est� apto a pegar
            if (newPlayer != null && !newPlayer.hasTail && newPlayer.CanGrabTail())
            {
                // Procura o jogador atual que possui a cauda (o que est� com a cauda agora)
                PlayerControllerTail currentOwner = transform.parent?.GetComponent<PlayerControllerTail>();

                // Se houver um jogador com a cauda, ele perde a cauda
                if (currentOwner != null)
                {
                    currentOwner.hasTail = false;
                    currentOwner.ActivateCooldown(); // Ativa o cooldown para o jogador que perdeu a cauda

                    // Libera a f�sica da cauda para que ela se mova livremente
                    Rigidbody currentOwnerRb = transform.GetComponent<Rigidbody>();
                    if (currentOwnerRb != null)
                    {
                        currentOwnerRb.isKinematic = false; // Reabilita a f�sica da cauda
                    }

                    // Desvincula a cauda do jogador atual
                    transform.SetParent(null); // Remove a cauda do jogador atual
                }

                // O novo jogador recebe a cauda
                newPlayer.hasTail = true;

                // Torna a cauda filha do novo jogador
                transform.SetParent(newPlayer.transform);
                transform.localPosition = new Vector3(0, 0.1f, -0.7f); // Ajusta a posi��o da cauda

                // Desativa a f�sica da cauda para que ela se mova com o novo jogador
                Rigidbody tailRb = transform.GetComponent<Rigidbody>();
                if (tailRb != null)
                {
                    tailRb.isKinematic = true; // Desabilita a f�sica para que a cauda siga o jogador
                }
            }
        }
    }
}