using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public enum ObstacleType { Moving, Rotating }
    public ObstacleType type;

    // Para objetos móveis
    public Vector3 moveDirection = Vector3.left; // Direção do movimento
    public float moveDistance = 5f; // Distância total de movimento
    public float moveSpeed = 2f; // Velocidade do movimento
    public bool isContinuousMovement = false; // Movimento contínuo

    private Vector3 startPosition;

    // Para objetos rotatórios
    public Vector3 rotationAxis = Vector3.up; // Eixo de rotação
    public float rotationSpeed = 50f; // Velocidade de rotação

    // Para a interferência no jogador
    public int pointsLossOnCollision = 10; // Quantidade de pontos que o jogador perde ao colidir

    void Start()
    {
        if (type == ObstacleType.Moving)
        {
            startPosition = transform.position;
        }
    }

    void Update()
    {
        if (type == ObstacleType.Moving)
        {
            MoveObstacle();
        }
        else if (type == ObstacleType.Rotating)
        {
            RotateObstacle();
        }
    }

    void MoveObstacle()
    {
        float offset;
        if (isContinuousMovement)
        {
            offset = Time.time * moveSpeed; // Movimento contínuo
        }
        else
        {
            offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance); // Movimento de ida e volta
        }

        transform.position = startPosition + moveDirection.normalized * offset;
    }

    void RotateObstacle()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    // Detecta colisão com o jogador
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto colidido é um jogador
        if (other.CompareTag("Player"))
        {
            PlayerControllerTail player = other.GetComponent<PlayerControllerTail>();
            if (player != null)
            {
                ApplyPointsLoss(player);
            }
        }
    }

    // Aplica a perda de pontos no jogador
    void ApplyPointsLoss(PlayerControllerTail player)
    {
        // Verifica se o jogador tem pontos suficientes para perder
        if (player != null)
        {
            player.LosePoints(pointsLossOnCollision); // Chama o método no script do jogador para perder pontos
        }
    }
}
