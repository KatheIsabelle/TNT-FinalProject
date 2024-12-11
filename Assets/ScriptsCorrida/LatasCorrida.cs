using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatasCorrida : MonoBehaviour
{
    public float tempoDeDestruicao = 5f;
    [HideInInspector]
    public GeradorLatasCorrida meuGerador;

    public float velocidadeFlutuacao = 1f; // Velocidade da flutua��o
    public float amplitudeFlutuacao = 0.3f; // Amplitude do movimento vertical
    public float velocidadeRotacao = 50f; // Velocidade da rota��o diagonal
    private Vector3 posicaoInicial;

    // Adicione uma vari�vel para identificar a lata �nica
    public bool eLataUnica = false;

    void Start()
    {
        // Define a inclina��o inicial na diagonal
        posicaoInicial = transform.position; // Guarda a posi��o inicial
        transform.rotation = Quaternion.Euler(0, 130, 0); // Inclina��o fixa de 45 graus no eixo X
        if (!eLataUnica)
        {
            Destroy(gameObject, tempoDeDestruicao);
        }
    }

    void Update()
    {
        // Apenas fa�a as anima��es se a lata n�o for �nica
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
        // Adiciona um movimento oscilat�rio vertical � lata
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
