using UnityEngine;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID único do jogador
    public float speed = 10f;
    public bool hasTail = false;

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
    }
}
