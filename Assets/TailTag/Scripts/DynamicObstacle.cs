using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public enum ObstacleType { Moving, Rotating }
    public ObstacleType type;

    // Para objetos móveis
    public Vector3 moveDirection = Vector3.left; // Direção do movimento
    public float moveDistance = 5f; // Distância total de movimento
    public float moveSpeed = 2f; // Velocidade do movimento

    private Vector3 startPosition;

    // Para objetos rotatórios
    public Vector3 rotationAxis = Vector3.up; // Eixo de rotação
    public float rotationSpeed = 50f; // Velocidade de rotação

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
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + moveDirection.normalized * offset;
    }

    void RotateObstacle()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
