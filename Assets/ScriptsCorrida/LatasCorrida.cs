using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatasCorrida : MonoBehaviour
{
    private float tempoDeDestruicao = 5f;
    [HideInInspector]
    public GeradorLatasCorrida meuGerador;

    private float velocidadeFlutuacao = 1f; // Velocidade da flutuação
    private float amplitudeFlutuacao = 0.3f; // Amplitude do movimento vertical
    private float velocidadeRotacao = 50f; // Velocidade da rotação diagonal
    private Vector3 posicaoInicial;

    void Start()
    {
        // Define a inclinação inicial na diagonal
        transform.rotation = Quaternion.Euler(45, 0, 0); // Inclinação fixa de 45 graus no eixo X
        posicaoInicial = transform.position; // Guarda a posição inicial
        Destroy(gameObject, tempoDeDestruicao);   
    }

    void Update()
    {
        Flutuar();
        RotacionarDiagonal();
    }

    private void RotacionarDiagonal()
    {
        // Rotaciona ao redor do eixo Y (de direita para esquerda)
        transform.Rotate(Vector3.up * velocidadeRotacao * Time.deltaTime, Space.World);
    }

    private void Flutuar()
    {
        // Adiciona um movimento oscilatório vertical à lata
        float deslocamentoY = Mathf.Sin(Time.time * velocidadeFlutuacao) * amplitudeFlutuacao;
        transform.position = new Vector3(posicaoInicial.x, posicaoInicial.y + deslocamentoY, posicaoInicial.z);
    }

    void OnTriggerEnter(Collider objetoDeColisao)
    {
        if (objetoDeColisao.tag == "Player")
        {
            meuGerador.DiminuirQuantidadeDeLatasNaPista();
            objetoDeColisao.GetComponent<PlayerControllerCorrida>().ColetarLatas();
            Destroy(gameObject);
        }
    }
}
