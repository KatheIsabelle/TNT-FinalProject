using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaQueda : MonoBehaviour
{
    private Vector3 posicaoInicial; // Posi��o inicial da plataforma
    public float tempoAntesDeCair = 1f; // Tempo que a plataforma espera antes de cair
    public float tempoAntesDeVoltar = 2f; // Tempo que a plataforma espera antes de retornar
    public float velocidadeDeRetorno = 2f; // Velocidade do retorno � posi��o inicial

    private Rigidbody rb; // Refer�ncia ao Rigidbody
    private bool emQueda = false; // Controle para evitar m�ltiplas ativa��es

    void Start()
    {
        // Salva a posi��o inicial da plataforma
        posicaoInicial = transform.position;

        // Obt�m o Rigidbody do GameObject
        rb = GetComponent<Rigidbody>();

        // Configura o Rigidbody para ser est�tico inicialmente
        rb.isKinematic = true; // A plataforma n�o ser� afetada pela f�sica at� ser ativada
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o jogador entrou em contato com a plataforma
        if (collision.collider.CompareTag("Player") && !emQueda)
        {
            emQueda = true; // Marca como ativada para evitar reativa��o
            StartCoroutine(PlataformaCair());
        }
    }

    private IEnumerator PlataformaCair()
    {
        // Aguarda o tempo antes de cair
        yield return new WaitForSeconds(tempoAntesDeCair);

        // Libera o Rigidbody para come�ar a cair
        rb.isKinematic = false;

        // Aguarda o tempo antes de come�ar a voltar
        yield return new WaitForSeconds(tempoAntesDeVoltar);

        // Volta suavemente � posi��o inicial
        StartCoroutine(VoltarParaPosicaoInicial());
    }

    private IEnumerator VoltarParaPosicaoInicial()
    {
        rb.isKinematic = true; // Desativa a f�sica
        rb.velocity = Vector3.zero; // Reseta qualquer movimento do Rigidbody

        float distancia = Vector3.Distance(transform.position, posicaoInicial);
        while (distancia > 0.01f) // Enquanto a plataforma n�o estiver perto o suficiente da posi��o inicial
        {
            transform.position = Vector3.Lerp(transform.position, posicaoInicial, Time.deltaTime * velocidadeDeRetorno);
            distancia = Vector3.Distance(transform.position, posicaoInicial);
            yield return null;
        }

        transform.position = posicaoInicial; // Garante a posi��o exata no final
        emQueda = false; // Permite que a plataforma seja ativada novamente
    }
}
