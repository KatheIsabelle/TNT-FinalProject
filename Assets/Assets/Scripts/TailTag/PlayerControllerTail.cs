using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID único do jogador
    public float speed = 10f;
    public bool hasTail = false;
    private float cooldownTimer = 0f; // Temporizador para impedir pegar a cauda imediatamente
    public float tailCooldown = 2f; // Tempo de cooldown em segundos
    //private float gravityScale = -9.81f;

    private Rigidbody rb;

    // Variáveis de pontuação
    public int points = 0; // Pontuação do jogador
    private float tailTime = 0f; // Tempo com a cauda

    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool dashed = false;


    private Animator animator; // Referência ao animator do player.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        transform.eulerAngles = new Vector3 (0f, 180f, 0f);
    }

    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        //jumped = context.ReadValue<bool>();
        jumped = context.action.triggered;
    }

    public void OnDash(InputAction.CallbackContext context) {
        //jumped = context.ReadValue<bool>();
        dashed = context.action.triggered;
    }

    void Update()
    {
        // Controle do jogador
        float moveHorizontal = movementInput.x;
        float moveVertical = movementInput.y;

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized * (hasTail ? speed + 3f : speed);  
        rb.velocity = new Vector3 (movement.x, rb.velocity.y, movement.z);
        float rotatespeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, movement, Time.deltaTime * rotatespeed);
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
        animator.SetBool("isMoving", movement != Vector3.zero);


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

    // Ao final do jogo, dá o bônus de 50 pontos se o jogador estiver com a cauda
    public void AddEndGamePoints()
    {
        if (hasTail)
        {
            points += 50; // Bônus de 50 pontos
        }
    }

    // Método para o jogador perder pontos ao colidir com o obstáculo
    public void LosePoints(int pointsToLose)
    {
        points -= pointsToLose;

        // Garante que a pontuação nunca fique abaixo de 0
        if (points < 0)
        {
            points = 0;
        }

        // Opcional: Aqui você pode adicionar alguma lógica para mostrar uma mensagem, som ou animação de perda de pontos
        Debug.Log($"{gameObject.name} perdeu {pointsToLose} pontos. Pontuação atual: {points}");
    }
}