using UnityEngine;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID �nico do jogador
    public float speed = 10f;
    public bool hasTail = false;
    private float cooldownTimer = 0f; // Temporizador para impedir pegar a cauda imediatamente
    public float tailCooldown = 2f; // Tempo de cooldown em segundos

    private Rigidbody rb;

    // Vari�veis de pontua��o
    public int points = 0; // Pontua��o do jogador
    private float tailTime = 0f; // Tempo com a cauda

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Controle do jogador baseado no ID
        float moveHorizontal = Input.GetAxis("Horizontal" + playerId);
        float moveVertical = Input.GetAxis("Vertical" + playerId);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        rb.velocity = movement * (hasTail ? speed + 3f : speed);

        // Atualizar o temporizador de cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Incrementar pontos se o jogador estiver com a cauda
        if (hasTail)
        {
            tailTime += Time.deltaTime;
            points = (int)(tailTime * 5); // 5 pontos por segundo com a cauda
        }
    }

    // M�todo para ativar o cooldown quando o jogador perde a cauda
    public void ActivateCooldown()
    {
        cooldownTimer = tailCooldown;
    }

    // Verifica se o jogador pode pegar a cauda
    public bool CanGrabTail()
    {
        return cooldownTimer <= 0;
    }

    // Ao final do jogo, d� o b�nus de 50 pontos se o jogador estiver com a cauda
    public void AddEndGamePoints()
    {
        if (hasTail)
        {
            points += 50; // B�nus de 50 pontos
        }
    }

    // M�todo para o jogador perder pontos ao colidir com o obst�culo
    public void LosePoints(int pointsToLose)
    {
        points -= pointsToLose;

        // Garante que a pontua��o nunca fique abaixo de 0
        if (points < 0)
        {
            points = 0;
        }
        Debug.Log($"{gameObject.name} perdeu {pointsToLose} pontos. Pontua��o atual: {points}");
    }
}