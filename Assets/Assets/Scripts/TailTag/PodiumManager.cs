using UnityEngine;
using TMPro;

public class PodiumManager : MonoBehaviour
{
    public Transform[] podiumPositions; // Posições no pódio (1º, 2º, 3º)
    public TMP_Text[] podiumTexts; // Lista de TMP_Text para os campos de texto do pódio (1º, 2º, 3º)

    void Start()
    {
        // Certifique-se de que os jogadores estão desabilitados para movimento
        GameObject[] playersInScene = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log(playersInScene[0]);

        foreach (var player in playersInScene)
        {
            // Verifica se o jogador está no ranking
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
                Destroy(player); // Destroi jogadores não presentes no ranking
            }
            else
            {
                // Desabilita movimento dos jogadores duplicados que não estão no pódio
                PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
                if (controller != null)
                {
                    controller.enabled = false; // Desabilita o movimento
                }
            }
        }

        // Organiza os jogadores no pódio
        for (int i = 0; i < GameData.playerRankings.Count; i++)
        {
            PlayerData playerData = GameData.playerRankings[i];
            GameObject player = playerData.playerObject;

            // Se o jogador tem cauda, remova-a antes de posicioná-lo no pódio
            if (player.GetComponent<PlayerControllerTail>().hasTail)
            {
                RemoveTailFromPlayer(player);
            }

            // Posiciona o jogador no pódio
            player.transform.position = podiumPositions[i].position;
            player.transform.rotation = podiumPositions[i].rotation;

            // Resetar a escala do jogador para a original
            player.transform.localScale = Vector3.one; // Reseta a escala para o tamanho original

            // Habilita o movimento apenas para os jogadores no pódio
            PlayerControllerTail controller = player.GetComponent<PlayerControllerTail>();
            if (controller != null)
            {
                controller.enabled = true;
            }
        }

        // Atualiza os campos de texto com as informações dos jogadores no pódio
        for (int i = 0; i < GameData.playerRankings.Count; i++)
        {
            PlayerData playerData = GameData.playerRankings[i];

            // Formata a string para exibir nome e pontos juntos
            string displayText = $"{playerData.playerName} \n {playerData.points} pontos";

            // Atualiza o campo de texto do pódio
            podiumTexts[i].text = displayText;
        }

        // Desabilita o controle dos jogadores no pódio (garante que não possam mais se mover)
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
