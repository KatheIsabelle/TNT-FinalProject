using System.Collections;
using UnityEngine;

/*
Script responsável por implementar eventos 
que podem ocorrer com o jogador durante o jogo, 
como morte e respawn.
*/


public class PlayerEvents : MonoBehaviour
{
    [Header("Player Components")]
    private Animator animator;
    private GameObject player;

    [Header("Death and Respawn Settings")]
    public bool isDead;
    private Vector3 respawnPoint;
    private GameObject respawnPointObject;

    [Header("UI")]
    public GameObject eliminatedCanvas;

    [Header("Layer Settings")]
    [SerializeField] private LayerMask deathPlatform;

    void Start()
    {
        player = gameObject;
        animator = GetComponent<Animator>();
        isDead = false;

        respawnPointObject = GameObject.FindGameObjectWithTag("Respawn");

        if (respawnPointObject != null)
        {
            SetRespawnPoint(respawnPointObject.transform.position);
        }
        else
        {
            Debug.LogError("Respawn point not found.");
        }
    }

    void Update()
    {
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (isDead) return;

        // Raycast para detectar a camada de morte
        if (Physics.Raycast(transform.position, Vector3.down, 1f, deathPlatform))
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        Debug.Log("Player morreu");

        // Mostra tela de eliminação, se configurada
        if (eliminatedCanvas != null)
            eliminatedCanvas.SetActive(true);

        StartCoroutine(Respawn());
    }

    private void SetRespawnPoint(Vector3 position)
    {
        respawnPoint = position;
        Debug.Log("Respawn point set to: " + respawnPoint);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);

        // Reposiciona e reinicia o jogador
        player.transform.position = respawnPoint;
        isDead = false;

        if (eliminatedCanvas != null)
            eliminatedCanvas.SetActive(false);

        animator.ResetTrigger("isDead");
        Debug.Log("Player respawned");
    }
}
