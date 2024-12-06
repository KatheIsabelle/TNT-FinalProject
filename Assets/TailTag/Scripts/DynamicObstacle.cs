using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public enum ObstacleType { Moving, Rotating }
    public ObstacleType type;

    // Para objetos m�veis
    public Vector3 moveDirection = Vector3.left; // Dire��o do movimento
    public float moveDistance = 5f; // Dist�ncia total de movimento
    public float moveSpeed = 2f; // Velocidade do movimento
    public bool isContinuousMovement = false; // Movimento cont�nuo

    private Vector3 startPosition;

    // Para objetos rotat�rios
    public Vector3 rotationAxis = Vector3.up; // Eixo de rota��o
    public float rotationSpeed = 50f; // Velocidade de rota��o

    // Intensidade do empurr�o
    public float pushForce = 100f; // Ajuste conforme necess�rio

    void Start()
    {
        startPosition = transform.position;

        if (type == ObstacleType.Moving)
        {
            moveDirection.Normalize(); // Garante que a dire��o esteja normalizada
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
            ? Mathf.Repeat(Time.time * moveSpeed, moveDistance) // Movimento cont�nuo
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
        // Obt�m o ponto de contato mais pr�ximo do obst�culo
        Vector3 contactPoint = collision.contacts[0].point;

        // Calcula a dire��o do empurr�o com base no ponto de contato
        Vector3 pushDirection = (collision.collider.transform.position - contactPoint).normalized;
        pushDirection.y = 0; // Garante que n�o empurre para cima

        // Aplica for�a ao Rigidbody do jogador
        playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}