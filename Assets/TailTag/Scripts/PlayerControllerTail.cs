using UnityEngine;

public class PlayerControllerTail : MonoBehaviour
{
    public int playerId; // ID �nico do jogador
    public float speed = 10f;
    public bool hasTail = false;
    public GameObject tailObject; // Refer�ncia para a cauda

    private Rigidbody rb;
    private Rigidbody tailRb; // Refer�ncia para o Rigidbody da cauda

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

            // Atualiza a posi��o da Tail para seguir o jogador
            tailObject = other.gameObject; // A cauda � o objeto com o qual colidimos
            tailRb = tailObject.GetComponent<Rigidbody>(); // Refer�ncia ao Rigidbody da cauda

            tailObject.transform.SetParent(this.transform); // Faz a cauda seguir o jogador
            tailObject.transform.localPosition = new Vector3(0, 0.5f, -0.5f); // Ajusta a posi��o da cauda

            // Desabilitar a f�sica da cauda (gravidade e colis�es)
            if (tailRb != null)
            {
                tailRb.isKinematic = true; // Desabilita a f�sica
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se o jogador saiu da colis�o com a cauda
        if (other.CompareTag("Tail") && hasTail)
        {
            Debug.Log($"{gameObject.name} perdeu a cauda!");
            RemoveTail(); // Chama o m�todo para remover a cauda
        }
    }

    // M�todo para remover a cauda (por exemplo, se o jogador perder a cauda)
    private void RemoveTail()
    {
        if (hasTail && tailObject != null)
        {
            // Restaura a f�sica da cauda quando � removida
            if (tailRb != null)
            {
                tailRb.isKinematic = false; // Habilita novamente a f�sica (gravidade)
            }

            tailObject.transform.SetParent(null); // Solta a cauda
            tailObject = null; // Limpa a refer�ncia � cauda
            hasTail = false; // Atualiza o estado para que o jogador n�o tenha mais cauda
            speed = 10f; // Restaura a velocidade para o valor padr�o
        }
    }
}
