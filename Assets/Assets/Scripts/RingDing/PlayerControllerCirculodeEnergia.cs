using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.InputSystem; 
using TMPro;

/// <summary>
/// Controlador do jogador para o modo Ring Ding.
/// Gerencia movimentação, pulos, ataques e interações com plataformas e itens.
/// </summary>
public class PlayerControllerCirculodeEnergia : MonoBehaviour // Define a classe principal do controlador do jogador.
{
    #region VARIÁVEIS DO PLAYER // Agrupamento de variáveis relacionadas ao jogador.

    [Header("Player Movimentos / Posicionamentos")] // Adiciona um cabeçalho no inspetor para organização.
    [Tooltip("Velocidade base do player")] // Dica exibida no inspetor sobre a variável.
    public float playerSpeed = 5f; // Velocidade de movimentação do jogador.

    [Tooltip("Força do pulo")] 
    public float jumpForce = 5f; // Define a força aplicada ao pular.

    [Tooltip("Limite de Queda")]
    public float fallThreshold; // Posição Y mínima antes de ativar o respawn.

    [Tooltip("Tempo para o Respawn")]
    public float timeThreshold; // Tempo de espera para reaparecer após cair.

    [Tooltip("Ponto de Respawn")]
    public Transform PointRespaw; // Posição para reaparecer no jogo.

    [Tooltip("Fator de desaceleração ao soltar as teclas")]
    public float decelerationFactor = 10f; // Controle de desaceleração ao parar de mover.

    public float pushForce = 10f; // Força aplicada ao jogador quando empurrado.

    private bool isRespawning = false; // Verifica se o jogador está no processo de respawn.
    private bool isGrounded = true; // Indica se o jogador está no chão.
    private bool isJumping; // Indica se o jogador está pulando.
    private bool canAttack = true; // Define se o jogador pode atacar.
    private bool jumped = false; // Registra o estado de pulo a partir da entrada.
    private bool attacked = false; // Registra o estado de ataque a partir da entrada.

    private float ySpeed; // Velocidade vertical do jogador.
    private Vector3 inputDirection; // Direção de movimento do jogador.
    private Vector2 movementInput = Vector2.zero; // Entrada de movimento do jogador.

    private Rigidbody rb; // Componente Rigidbody do jogador para física.
    private Transform currentPlatform = null; // Plataforma atual em que o jogador está.

    public static PlayerControllerCirculodeEnergia Instance; // Instância singleton da classe.

    #endregion

    #region REFERÊNCIAS // Variáveis para objetos e scripts auxiliares.

    private GameControllerCirculodeEnergia gameController; // Referência ao controlador do jogo.
    private int playerID; // ID único do jogador registrado no controlador.
    public TextMeshProUGUI playerScoreText;  // Referência ao TMP de pontuação do jogador
    private Animator animator; // Referência ao animator do player.
    public GameObject attackZone;
    #endregion

    #region MÉTODOS UNITY // Métodos principais do ciclo de vida do Unity.

    /// <summary>
    /// Configura a instância singleton do PlayerController.
    /// </summary>
    void Awake()
    {
        Instance = this; // Define a instância singleton para acesso global.
    }

    /// <summary>
    /// Inicializa referências e registra o jogador no GameController.
    /// </summary>
    void Start()
    {
        // Obtém a instância do GameController
        gameController = GameControllerCirculodeEnergia.Instance;

        // Obtém o Rigidbody do jogador
        rb = GetComponent<Rigidbody>(); 

        // Impede a rotação do Rigidbody
        rb.freezeRotation = true;  
       
        // Registra o jogador no GameController e associa o TMP de pontuação
        playerID = gameController.RegisterPlayer(playerScoreText);  // Passa o TMP ao registrar o jogador

        // Obtém o animator do jogador
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Atualiza lógica de pulo, queda e plataforma a cada frame.
    /// </summary>
    void Update()
    {
        // Aplica a gravidade continuamente.
        ySpeed += Physics.gravity.y * Time.deltaTime; 

        // Chama a lógica de pulo.
        Jump(); 

        // Atualiza o estado de queda.
        Falling(); 

        // Verifica a plataforma atual.
        CheckCurrentPlatform(); 
    }

    /// <summary>
    /// Atualiza a lógica de movimentação e respawn em intervalos fixos.
    /// </summary>
    void FixedUpdate()
    {
        // Chama a lógica de movimentação.
        Move(); 

        // Verifica se caiu abaixo do limite.
        if (transform.position.y < fallThreshold && !isRespawning) 
        {
            isRespawning = true; // Marca como em respawn.
            StartCoroutine(Respawn()); // Inicia o processo de respawn.
        }

        // Gerencia o ataque em corrotina.
        StartCoroutine(Attack()); 
    }

    /// <summary>
    /// Detecta colisões com o chão e plataformas.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground") || // Verifica tags de chão e plataformas.
            collision.collider.CompareTag("Platforms/PlatformStop") ||
            collision.collider.CompareTag("Platforms/Rotating_1") ||
            collision.collider.CompareTag("Platforms/Rotating_2"))
        {
            ySpeed = -0.5f; // Redefine a velocidade vertical para simular aterrissagem.
            isGrounded = true; // Marca o jogador como no chão.
            isJumping = false; // Define que não está mais pulando.
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    /// <summary>
    /// Detecta a coleta de itens e concede pontos ao jogador.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        int points = 0; // Inicializa os pontos para a coleta.

        // Verifica os diferentes itens coletáveis.
        if (other.CompareTag("YellowCan")) points = 2; 
        else if (other.CompareTag("PynkCan")) points = 2;
        else if (other.CompareTag("BlueCan")) points = 4;
        else if (other.CompareTag("RedCan")) points = 1;
        else if (other.CompareTag("GreenCan")) points = 3;

        // Se coletou algo, adiciona os pontos.
        if (points > 0) 
        {
            gameController.AddScore(playerID, points); // Adiciona a pontuação ao jogador.
            Destroy(other.gameObject); // Remove o item coletado da cena.
        }
    }

    #endregion

    #region MÉTODOS DE CONTROLE // Métodos responsáveis pelas entradas de controle do jogador.

    /// <summary>
    /// Atualiza a entrada de movimentação do jogador.
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>(); // Lê o valor do input de movimentação (teclas ou controle).
    }

    /// <summary>
    /// Verifica se o jogador acionou o pulo.
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered; // Registra se o pulo foi acionado.
    }

    /// <summary>
    /// Verifica se o jogador acionou o ataque.
    /// </summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        attacked = context.action.triggered; // Registra se o ataque foi acionado.
    }

    /// <summary>
    /// Controla a movimentação do jogador com base na entrada do teclado.
    /// </summary>
    void Move()
    {
        float horizontal = movementInput.x; // Obtém a entrada horizontal (esquerda/direita).
        float vertical = movementInput.y; // Obtém a entrada vertical (cima/baixo).
        inputDirection = new Vector3(horizontal, 0, vertical).normalized; // Cria a direção normalizada de movimento.

        if (inputDirection.magnitude > 0) // Se há movimento, aplica a velocidade.
        {
            Vector3 targetVelocity = inputDirection * playerSpeed; // Calcula a velocidade alvo.
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z); // Aplica a velocidade no Rigidbody, mantendo a velocidade Y (vertical) atual.
        }
        else // Se não houver movimento, aplica desaceleração.
        {
            Vector3 currentVelocity = rb.velocity; // Obtém a velocidade atual.
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, 0, decelerationFactor * Time.fixedDeltaTime); // Desacelera horizontalmente.
            currentVelocity.z = Mathf.Lerp(currentVelocity.z, 0, decelerationFactor * Time.fixedDeltaTime); // Desacelera verticalmente.
            rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z); // Atualiza a velocidade do Rigidbody.
        }

        float rotatespeed = 10f; // Velocidade de rotação para suavizar a orientação do jogador.
        transform.forward = Vector3.Slerp(transform.forward, inputDirection, Time.deltaTime * rotatespeed); // Gira suavemente o jogador para a direção de movimento.
        
        if (isJumping) { 
            animator.SetBool("isMoving", false); 
        } else {
            animator.SetBool("isMoving", inputDirection != Vector3.zero); 
        }
    }

    /// <summary>
    /// Executa o pulo do jogador caso seja permitido.
    /// </summary>
    void Jump()
    {
        if (jumped && isGrounded) // Se o jogador pressionou pulo e está no chão.
        {
            ySpeed = jumpForce; // Reseta a velocidade vertical para o valor do pulo.
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Aplica uma força de impulso para pular.
            isGrounded = false; // Marca o jogador como não estando mais no chão.
            float rotatespeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, inputDirection, Time.deltaTime * rotatespeed);
            animator.SetBool("isJumping", true);
            isJumping = true; // Marca o jogador como estando no ar (pulando).
            animator.SetBool("isGrounded", false);
            animator.SetBool("isMoving", false);
        }
    }

    /// <summary>
    /// Gerencia a transição entre estados de pulo e queda.
    /// </summary>
    void Falling()
    {
        if ((isJumping && ySpeed < 0) || (ySpeed < -2 && !isGrounded)) // Se o jogador está caindo.
        {
            animator.SetBool("isJumping", false);
            isJumping = false; // Marca como não estando mais pulando.
            animator.SetBool("isFalling", true);
        }
    }

    /// <summary>
    /// Controla o comportamento de ataque do jogador.
    /// </summary>
    private IEnumerator Attack()
    {
        if (attacked && canAttack) // Se o jogador acionou o ataque e está pronto.
        {
            animator.SetBool("isGrounded", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isAttacking", true);
            canAttack = false; // Impede ataques consecutivos imediatos.
            attackZone.SetActive(true);
            yield return new WaitForSeconds(0.3f); // Aguarda 0.3 segundos para a animação de ataque.
            animator.SetBool("isAttacking", false);
            attackZone.SetActive(false);
            yield return new WaitForSeconds(2.5f); // Aguarda o tempo necessário antes de permitir outro ataque.
            canAttack = true; // Permite outro ataque após o tempo de espera.
        }
    }

    /// <summary>
    /// Realiza o respawn do jogador após cair abaixo do limite.
    /// </summary>
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(timeThreshold); // Aguarda o tempo de respawn.
        transform.position = PointRespaw.position + Vector3.up * 3f; // Move o jogador para o ponto de respawn com um pequeno ajuste de altura.
        isRespawning = false; // Marca como não em respawn.
    }

    #endregion

    #region MÉTODOS DE PLATAFORMA // Métodos para lidar com plataformas interativas.

    /// <summary>
    /// Verifica e atualiza a plataforma atual do jogador.
    /// </summary>
    void CheckCurrentPlatform()
    {
        RaycastHit hit; // Define uma variável para armazenar os resultados do raycast.

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) // Verifica a plataforma abaixo do jogador usando um raycast.
        {
            if (hit.collider.CompareTag("Platforms/Rotating_1") ||
                hit.collider.CompareTag("Platforms/Rotating_2") ||
                hit.collider.CompareTag("Platforms/PlatformStop")) // Verifica se a plataforma é rotacionada ou parada.
            {
                if (currentPlatform != hit.transform) // Se a plataforma detectada não for a mesma que a atual.
                {
                    SetPlatform(hit.transform); // Atualiza a plataforma atual.
                }
            }
            else
            {
                ResetPlatform(); // Se não houver uma plataforma válida, redefine a plataforma atual.
            }
        }
        else
        {
            ResetPlatform(); // Se não houver nenhuma plataforma detectada, redefine a plataforma.
        }
    }

    /// <summary>
    /// Define a nova plataforma sob o jogador.
    /// </summary>
    void SetPlatform(Transform newPlatform)
    {
        currentPlatform = newPlatform; // Define a nova plataforma.
        transform.parent = newPlatform; // Torna o jogador filho da plataforma para se mover com ela.
    }

    /// <summary>
    /// Redefine a plataforma atual do jogador para nula.
    /// </summary>
    void ResetPlatform()
    {
        if (currentPlatform != null) // Se há uma plataforma atribuída.
        {
            currentPlatform = null; // Limpa a plataforma.
            transform.parent = null; // Remove o jogador de ser filho de qualquer plataforma.
        }
    }

    #endregion

    void Cheer() 
    {
        if (Input.GetButtonDown("Fire1")) {
            animator.SetBool("isMoving", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isCheering", true);
        }
    }

    #region VAI USAR?

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

    #endregion
}
