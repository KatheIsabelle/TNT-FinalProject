using UnityEngine;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID único do jogador
    public float speed = 10f;
    public bool hasTail = false;
    private float cooldownTimer = 0f; // Temporizador para impedir pegar a cauda imediatamente
    public float tailCooldown = 2f; // Tempo de cooldown em segundos

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Controle do jogador baseado no ID
        float moveHorizontal = Input.GetAxis("Horizontal" + playerId);
        float moveVertical = Input.GetAxis("Vertical" + playerId);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * (hasTail ? speed + 3f : speed);

        // Atualizar o temporizador de cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // Método para ativar o cooldown quando o jogador perde a cauda
    public void ActivateCooldown()
    {
        cooldownTimer = tailCooldown;
    }

    // Verifica se o jogador pode pegar a cauda
    public bool CanGrabTail()
    {
        return cooldownTimer <= 0;
    }
}
