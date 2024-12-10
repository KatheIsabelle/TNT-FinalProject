using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID �nico do jogador
    public float speed = 10f;
    public bool hasTail = false;
    private float cooldownTimer = 0f; // Temporizador para impedir pegar a cauda imediatamente
    public float tailCooldown = 2f; // Tempo de cooldown em segundos
    //private float gravityScale = -9.81f;

    private Rigidbody rb;

    // Vari�veis de pontua��o
    public int points = 0; // Pontua��o do jogador
    private float tailTime = 0f; // Tempo com a cauda

    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool dashed = false;


    private Animator animator; // Refer�ncia ao animator do player.

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

        // Opcional: Aqui voc� pode adicionar alguma l�gica para mostrar uma mensagem, som ou anima��o de perda de pontos
        Debug.Log($"{gameObject.name} perdeu {pointsToLose} pontos. Pontua��o atual: {points}");
    }
}