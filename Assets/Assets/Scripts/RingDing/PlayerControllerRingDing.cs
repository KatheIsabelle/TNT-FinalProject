using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerRingDing : MonoBehaviour
{
    #region VAR PLAYER
    [Header("Player Movimentos / Posicionamentos")]

    [Tooltip("Velocidade base do player")]
    public float playerSpeed = 5f; // Define a velocidade de movimento do player.

    [Tooltip("Força do pulo")]
    public float jumpForce = 5f; // Define a força que o player usa para pular.

    [Tooltip("Limite de Queda")]
    public float fallThreshold; // Altura mínima antes de o player ser respawnado.

    [Tooltip("Tempo para o Respawn")]
    public float timeThreshold; // Tempo para o player ser respawnado.

    [Tooltip("Ponto de Respawn")]
    public Transform PointRespaw; // Posição onde o player será colocado após cair.

    // Indica se o player está em processo de respawn.
    private bool isRespawning = false; 

    // Referência ao Rigidbody, usado para aplicar física no player.
    private Rigidbody rb; 

    // Indica se o player está no chão.
    private bool isGrounded = true; 

    [Tooltip("Fator de desaceleração ao soltar as teclas")]
    public float decelerationFactor = 10f; 

    // Armazena a direção do movimento com base na entrada do jogador.
    private Vector3 inputDirection; 

    // Força do empurrão
    public float pushForce = 10f; 

    // Variável para rastrear a plataforma atual
    private Transform currentPlatform = null;

    private bool isJumping;

    private float ySpeed;

    public GameObject attackZone;

    private bool canAttack = true;


    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;
    private bool attacked = false;
    #endregion

    public static PlayerControllerRingDing Instance; // Instância para acessar este script de outros lugares.
    private GameController gameController; // Referência ao controlador principal do jogo.
    private Animator animator; // Referência ao animator do player.
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameController = GameController.Instance;
        rb = GetComponent<Rigidbody>(); // Obtém o Rigidbody anexado ao objeto.
        rb.freezeRotation = true; // Evita rotações indesejadas no player.
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        //jumped = context.ReadValue<bool>();
        jumped = context.action.triggered;
    }

    public void OnAttack(InputAction.CallbackContext context) {
        //jumped = context.ReadValue<bool>();
        attacked = context.action.triggered;
    }

    void Update() 
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;
        Jump();
        Falling();
        CheckCurrentPlatform();
        //Cheer();
    }

    void FixedUpdate()
    {
        Move();

        // Checa se o jogador caiu abaixo do limite e deve ser respawnado.
        if (transform.position.y < fallThreshold && !isRespawning)
        {
            isRespawning = true;
            StartCoroutine(Respawn());
        }
        StartCoroutine(Attack());
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o player tocou o chão ou Plataformas.
        if (collision.collider.CompareTag("Ground") 
            || collision.collider.CompareTag("Platforms/PlatformStop")
            || collision.collider.CompareTag("Platforms/Rotating_1")
            || collision.collider.CompareTag("Platforms/Rotating_2"))
        {
            ySpeed = -0.5f;
            isGrounded = true; // Permite o próximo pulo.
            isJumping = false;
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("YellowCan"))
        {
            gameController.qtdCans++;

            Destroy(other.gameObject);

            // Chama a função para atualizar o contador no GameController
            gameController.OnCanDestroyed();
        }

        if(other.CompareTag("PynkCan"))
        {
            gameController.qtdCans++;

            Destroy(other.gameObject);

            // Chama a função para atualizar o contador no GameController
            gameController.OnCanDestroyed();
        }

        if(other.CompareTag("BlueCan"))
        {
            gameController.qtdCans++;

            Destroy(other.gameObject);

            // Chama a função para atualizar o contador no GameController
            gameController.OnCanDestroyed();
        }

        if(other.CompareTag("RedCan"))
        {
            gameController.qtdCans++;

            Destroy(other.gameObject);

            // Chama a função para atualizar o contador no GameController
            gameController.OnCanDestroyed();
        }

        if(other.CompareTag("GreenCan"))
        {
            gameController.qtdCans++;

            Destroy(other.gameObject);

            // Chama a função para atualizar o contador no GameController
            gameController.OnCanDestroyed();
        }
    }

  /*  private void OnCollisionStay(Collision collision)
    {
        // Verifica se a tecla B está sendo pressionada
        if (Input.GetKey(KeyCode.B))
        {
            // Verifica se o objeto colidido tem o componente Rigidbody
            Rigidbody otherRigidbody = collision.collider.GetComponent<Rigidbody>();

            if (otherRigidbody != null && collision.gameObject.CompareTag("Player"))
            {
                // Calcula a direção do empurrão
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;

                // Aplica a força no outro jogador
                otherRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
    }
  */
    private IEnumerator Attack() {
        if (attacked && canAttack) {
            animator.SetBool("isGrounded", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isAttacking", true);
            canAttack = false;
            attackZone.SetActive(true);
            yield return new WaitForSeconds(.3f);
            animator.SetBool("isAttacking", false);
            attackZone.SetActive(false);
            yield return new WaitForSeconds(2.5f);
            canAttack = true;
        }

    }

    void Move()
    {
        // Captura a entrada do teclado (valores de -1 a 1 para cada eixo).
        float horizontal = movementInput.x; // Entrada horizontal (A/D ou setas).
        float vertical = movementInput.y; // Entrada vertical (W/S ou setas).

        // Define a direção do movimento com base na entrada do jogador.
        inputDirection = new Vector3(horizontal, 0, vertical).normalized;
        // Normaliza para evitar que a magnitude ultrapasse 1 ao mover na diagonal.

        if (inputDirection.magnitude > 0) // Se houver entrada do jogador...
        {
            // Calcula a velocidade alvo com base na direção de entrada e na velocidade do player.
            Vector3 targetVelocity = inputDirection * playerSpeed;

            // Define a velocidade do Rigidbody, mantendo o componente vertical intacto.
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
        }
        else // Quando não há entrada...
        {
            // Obtém a velocidade atual do Rigidbody.
            Vector3 currentVelocity = rb.velocity;

            // Suaviza a desaceleração no eixo X.
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, 0, decelerationFactor * Time.fixedDeltaTime);

            // Suaviza a desaceleração no eixo Z.
            currentVelocity.z = Mathf.Lerp(currentVelocity.z, 0, decelerationFactor * Time.fixedDeltaTime);

            // Atualiza a velocidade do Rigidbody com a nova desaceleração aplicada.
            rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);
        }
        float rotatespeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, inputDirection, Time.deltaTime * rotatespeed);
        if (isJumping) { 
            animator.SetBool("isMoving", false); 
        } else {
            animator.SetBool("isMoving", inputDirection != Vector3.zero); 
        }
    }

    void Jump()
    {
        // Checa se o jogador pressionou o botão de pular e está no chão.
        if (jumped && isGrounded)
        {
            ySpeed = jumpForce;
            // Adiciona uma força vertical para o pulo.
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Marca que o jogador não está mais no chão.
            isGrounded = false;
            float rotatespeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, inputDirection, Time.deltaTime * rotatespeed);
            animator.SetBool("isJumping", true);
            isJumping = true;
            animator.SetBool("isGrounded", false);
            animator.SetBool("isMoving", false);
        }
    }

    void Falling() 
    {
        if ((isJumping && ySpeed < 0) || (ySpeed < -2 && !isGrounded)){
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", true);
        }
    }

    void Cheer() 
    {
        if (Input.GetButtonDown("Fire1")) {
            animator.SetBool("isMoving", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isCheering", true);
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeThreshold);

        // Move o player para o ponto de respawn.
        transform.position = PointRespaw.position + Vector3.up * 3f;
        Debug.Log("Respawn efetuado na posição: " + PointRespaw.position);
        isRespawning = false; // Reseta o estado de respawn.
    }

    // Método para verificar em qual plataforma o jogador está
    void CheckCurrentPlatform()
    {
        // Faz um raio (raycast) para baixo a partir do jogador

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) {
                // Confere se o objeto abaixo é uma plataforma válida
                if (hit.collider.CompareTag("Platforms/Rotating_1") ||
                    hit.collider.CompareTag("Platforms/Rotating_2") ||
                    hit.collider.CompareTag("Platforms/PlatformStop")) {
                    if (currentPlatform != hit.transform) {
                        SetPlatform(hit.transform); // Define a nova plataforma
                    }
                } else {
                    ResetPlatform(); // Reseta se não estiver sobre uma plataforma
                }
            } else {
                ResetPlatform(); // Reseta se não houver colisão abaixo
            }

    }

    // Define a plataforma atual
    void SetPlatform(Transform newPlatform)
    {
        currentPlatform = newPlatform; // Salva a nova plataforma
        transform.parent = newPlatform; // Torna o jogador filho da plataforma
        //isGrounded = true; // Permite o próximo pulo
    }

    // Remove vínculo com a plataforma
    void ResetPlatform()
    {
        if (currentPlatform != null)
        {
            currentPlatform = null; // Remove a referência
            transform.parent = null; // Reseta o vínculo de parentesco
        }
    }
}
