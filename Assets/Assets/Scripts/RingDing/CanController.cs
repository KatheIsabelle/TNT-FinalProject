using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanController : MonoBehaviour
{
    [Header("Latinha")]
    [Tooltip("Velocidade de flutuação da latinha")]
    public float floatSpeed = 0.1f;  
    [Tooltip("Tempo máximo antes da latinha estourar")]
    public float maxTimeBeforePop = 4f;  

    // Temporizador para o tempo de vida da latinha
    private float timeAlive = 0f;
    private void Start() {
        transform.eulerAngles = new Vector3(-90f,150,0);
    }
    void Update()
    {
        // Movimenta a latinha para cima, aplicando uma força constante para simular a flutuação
        transform.Translate(Vector3.down * floatSpeed * Time.deltaTime);

        // Atualiza o tempo de vida da latinha
        timeAlive += Time.deltaTime;

        // Verifica se a latinha ultrapassou o limite de 4 segundos
        if (timeAlive >= maxTimeBeforePop)
        {
            // Estoura a latinha
            PopCan();  
        }
    }

    // Função para estourar a latinha
    void PopCan()
    {
        // Remove a latinha da cena
        Destroy(gameObject);  

        // Atualiza o contador de latinhas no GameController
        GameController.Instance.OnCanDestroyed();  
    }
}
