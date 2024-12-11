using UnityEngine;
using TMPro;

public class PodiumManager : MonoBehaviour
{
    public Transform[] podiumPositions; // Posi��es no p�dio (1�, 2�, 3�)
    public TMP_Text[] podiumTexts; // Lista de TMP_Text para os campos de texto do p�dio (1�, 2�, 3�)

    void Start()
    {
        // Certifique-se de que os jogadores est�o desabilitados para movimento
        GameObject[] playersInScene = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log(playersInScene[0]);

        foreach (var player in playersInScene)
        {
            // Verifica se o jogador est� no ranking
            bool isPlayerInRanking = false;
            foreach (var playerData in GameData.playerRankings)
            {
                if (playerData.playerObject == player)
                {
                    isPlayerInRanking = true;
                    break;
                }
            }

            if (!isPlayerInRanking)
            {
                Destroy(player); // Destroi jogadores n�o presentes no ranking
            }
            else
            {
                // Desabilita movimento dos jogadores duplicados que n�o est�o no p�dio
                PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
                if (controller != null)
                {
                    controller.enabled = false; // Desabilita o movimento
                }
            }
        }

        // Organiza os jogadores no p�dio
        for (int i = 0; i < GameData.playerRankings.Count; i++)
        {
            PlayerData playerData = GameData.playerRankings[i];
            GameObject player = playerData.playerObject;

            // Se o jogador tem cauda, remova-a antes de posicion�-lo no p�dio
            if (player.GetComponent<PlayerControllerTail>().hasTail)
            {
                RemoveTailFromPlayer(player);
            }

            // Posiciona o jogador no p�dio
            player.transform.position = podiumPositions[i].position;
            player.transform.rotation = podiumPositions[i].rotation;

            // Resetar a escala do jogador para a original
            player.transform.localScale = Vector3.one; // Reseta a escala para o tamanho original

            // Habilita o movimento apenas para os jogadores no p�dio
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            if (controller != null)
            {
                controller.enabled = true;
            }
        }

        // Atualiza os campos de texto com as informa��es dos jogadores no p�dio
        for (int i = 0; i < GameData.playerRankings.Count; i++)
        {
            PlayerData playerData = GameData.playerRankings[i];

            // Formata a string para exibir nome e pontos juntos
            string displayText = $"{playerData.playerName} \n {playerData.points} pontos";

            // Atualiza o campo de texto do p�dio
            podiumTexts[i].text = displayText;
        }

        // Desabilita o controle dos jogadores no p�dio (garante que n�o possam mais se mover)
        foreach (var player in playersInScene)
        {
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            if (controller != null)
            {
                controller.enabled = false; // Desabilita o movimento
            }

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    void RemoveTailFromPlayer(GameObject player)
    {
        Transform tail = player.transform.Find("Tail");
        if (tail != null)
        {
            Destroy(tail.gameObject); // Remova a cauda do jogador
        }
    }
}
