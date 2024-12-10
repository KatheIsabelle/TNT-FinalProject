using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaQueda : MonoBehaviour
{
    private Vector3 posicaoInicial; // Posição inicial da plataforma
    public float tempoAntesDeCair = 1f; // Tempo que a plataforma espera antes de cair
    public float tempoAntesDeVoltar = 2f; // Tempo que a plataforma espera antes de retornar
    public float velocidadeDeRetorno = 2f; // Velocidade do retorno à posição inicial

    private Rigidbody rb; // Referência ao Rigidbody
    private bool emQueda = false; // Controle para evitar múltiplas ativações

    void Start()
    {
        // Salva a posição inicial da plataforma
        posicaoInicial = transform.position;

        // Obtém o Rigidbody do GameObject
        rb = GetComponent<Rigidbody>();

        // Configura o Rigidbody para ser estático inicialmente
        rb.isKinematic = true; // A plataforma não será afetada pela física até ser ativada
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o jogador entrou em contato com a plataforma
        if (collision.collider.CompareTag("Player") && !emQueda)
        {
            emQueda = true; // Marca como ativada para evitar reativação
            StartCoroutine(PlataformaCair());
        }
    }

    private IEnumerator PlataformaCair()
    {
        // Aguarda o tempo antes de cair
        yield return new WaitForSeconds(tempoAntesDeCair);

        // Libera o Rigidbody para começar a cair
        rb.isKinematic = false;

        // Aguarda o tempo antes de começar a voltar
        yield return new WaitForSeconds(tempoAntesDeVoltar);

        // Volta suavemente à posição inicial
        StartCoroutine(VoltarParaPosicaoInicial());
    }

    private IEnumerator VoltarParaPosicaoInicial()
    {
        rb.isKinematic = true; // Desativa a física
        rb.velocity = Vector3.zero; // Reseta qualquer movimento do Rigidbody

        float distancia = Vector3.Distance(transform.position, posicaoInicial);
        while (distancia > 0.01f) // Enquanto a plataforma não estiver perto o suficiente da posição inicial
        {
            transform.position = Vector3.Lerp(transform.position, posicaoInicial, Time.deltaTime * velocidadeDeRetorno);
            distancia = Vector3.Distance(transform.position, posicaoInicial);
            yield return null;
        }

        transform.position = posicaoInicial; // Garante a posição exata no final
        emQueda = false; // Permite que a plataforma seja ativada novamente
    }
}
