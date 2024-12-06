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

    // Intensidade do empurrão
    public float pushForce = 100f; // Ajuste conforme necessário

    void Start()
    {
        startPosition = transform.position;

        if (type == ObstacleType.Moving)
        {
            moveDirection.Normalize(); // Garante que a direção esteja normalizada
        }
    }

    void Update()
    {
        switch (type)
        {
            case ObstacleType.Moving:
                MoveObstacle();
                break;
            case ObstacleType.Rotating:
                RotateObstacle();
                break;
        }
    }

    void MoveObstacle()
    {
        float offset = isContinuousMovement
            ? Mathf.Repeat(Time.time * moveSpeed, moveDistance) // Movimento contínuo
            : Mathf.PingPong(Time.time * moveSpeed, moveDistance); // Movimento de ida e volta

        transform.position = startPosition + moveDirection * offset;
    }

    void RotateObstacle()
    {
        transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                PushPlayer(collision, playerRb);
            }
        }
    }

    void PushPlayer(Collision collision, Rigidbody playerRb)
    {
        // Obtém o ponto de contato mais próximo do obstáculo
        Vector3 contactPoint = collision.contacts[0].point;

        // Calcula a direção do empurrão com base no ponto de contato
        Vector3 pushDirection = (collision.collider.transform.position - contactPoint).normalized;
        pushDirection.y = 0; // Garante que não empurre para cima

        // Aplica força ao Rigidbody do jogador
        playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}