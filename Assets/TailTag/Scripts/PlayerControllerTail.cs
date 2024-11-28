using UnityEngine;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID único do jogador
    public float speed = 10f;
    public bool hasTail = false;
    public GameObject tailObject; // Referência para a cauda

    private Rigidbody rb;
    private Rigidbody tailRb; // Referência para o Rigidbody da cauda

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

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o jogador colidiu com a Tail
        if (other.CompareTag("Tail") && !hasTail)
        {
            Debug.Log($"{gameObject.name} pegou a cauda!");
            hasTail = true;

            // Atualiza a posição da Tail para seguir o jogador
            tailObject = other.gameObject; // A cauda é o objeto com o qual colidimos
            tailRb = tailObject.GetComponent<Rigidbody>(); // Referência ao Rigidbody da cauda

            tailObject.transform.SetParent(this.transform); // Faz a cauda seguir o jogador
            tailObject.transform.localPosition = new Vector3(0, 0.5f, -0.5f); // Ajusta a posição da cauda

            // Desabilitar a física da cauda (gravidade e colisões)
            if (tailRb != null)
            {
                tailRb.isKinematic = true; // Desabilita a física
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se o jogador saiu da colisão com a cauda
        if (other.CompareTag("Tail") && hasTail)
        {
            Debug.Log($"{gameObject.name} perdeu a cauda!");
            RemoveTail(); // Chama o método para remover a cauda
        }
    }

    // Método para remover a cauda (por exemplo, se o jogador perder a cauda)
    private void RemoveTail()
    {
        if (hasTail && tailObject != null)
        {
            // Restaura a física da cauda quando é removida
            if (tailRb != null)
            {
                tailRb.isKinematic = false; // Habilita novamente a física (gravidade)
            }

            tailObject.transform.SetParent(null); // Solta a cauda
            tailObject = null; // Limpa a referência à cauda
            hasTail = false; // Atualiza o estado para que o jogador não tenha mais cauda
            speed = 10f; // Restaura a velocidade para o valor padrão
        }
    }
}
