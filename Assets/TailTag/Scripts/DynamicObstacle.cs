using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    public enum ObstacleType { Moving, Rotating }
    public ObstacleType type;

    // Para objetos m�veis
    public Vector3 moveDirection = Vector3.left; // Dire��o do movimento
    public float moveDistance = 5f; // Dist�ncia total de movimento
    public float moveSpeed = 2f; // Velocidade do movimento

    private Vector3 startPosition;

    // Para objetos rotat�rios
    public Vector3 rotationAxis = Vector3.up; // Eixo de rota��o
    public float rotationSpeed = 50f; // Velocidade de rota��o

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
