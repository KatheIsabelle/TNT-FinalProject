using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatasCorrida : MonoBehaviour
{
    public float tempoDeDestruicao = 5f;
    [HideInInspector]
    public GeradorLatasCorrida meuGerador;

    public float velocidadeFlutuacao = 1f; // Velocidade da flutuação
    public float amplitudeFlutuacao = 0.3f; // Amplitude do movimento vertical
    public float velocidadeRotacao = 50f; // Velocidade da rotação diagonal
    private Vector3 posicaoInicial;

    // Adicione uma variável para identificar a lata única
    public bool eLataUnica = false;

    void Start()
    {
        // Define a inclinação inicial na diagonal
        posicaoInicial = transform.position; // Guarda a posição inicial
        transform.rotation = Quaternion.Euler(0, 130, 0); // Inclinação fixa de 45 graus no eixo X
        if (!eLataUnica)
        {
            Destroy(gameObject, tempoDeDestruicao);
        }
    }

    void Update()
    {
        // Apenas faça as animações se a lata não for única
        if (!eLataUnica)
        {
            Flutuar();
            RotacionarDiagonal();
        }
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
            objetoDeColisao.GetComponent<PlayerControllerCorridaCaminhoEnergizado>().ColetarLatas();
            Destroy(gameObject);
        }
    }
}
