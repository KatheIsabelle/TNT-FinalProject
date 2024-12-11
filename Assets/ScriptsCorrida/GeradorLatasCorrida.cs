using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeradorLatasCorrida : MonoBehaviour
{
    public GameObject Latas;
    float contadorTempo = 0f;
    public float tempoGeradorLatas = 1f;
    public LayerMask LayerLata;
    public float distanciaDeGeracao = 3f;
    public float distanciaDoJogadorParaGeracao = 10f;
    private GameObject jogador;
    private float quantidadeMaximaDeLatasNaPista = 6f;
    private float quantidadeDeLatasNaPista;

    // Variável para controlar geração única
    public bool gerarApenasUmaLata = false; // Ativar/desativar a geração única
    private bool lataUnicaGerada = false; // Flag para verificar se a única lata já foi gerada

    void Start()
    {
        jogador = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        bool possoGerarLatasDistancia = Vector3.Distance(transform.position, jogador.transform.position) <= distanciaDoJogadorParaGeracao;

        // Caso esteja configurado para gerar apenas uma lata
        if (gerarApenasUmaLata && possoGerarLatasDistancia)
        {
            if (!lataUnicaGerada)
            {
                StartCoroutine(GerarUmaUnicaLata());
                lataUnicaGerada = true; // Marca que já foi gerada
            }
            return; // Sai do método para evitar gerar mais latas
        }

        if (possoGerarLatasDistancia && quantidadeDeLatasNaPista < quantidadeMaximaDeLatasNaPista)
        {
            
            contadorTempo += Time.deltaTime;

            if (contadorTempo >= tempoGeradorLatas)
            {
                StartCoroutine(GerarUmaNovaLata());
                contadorTempo = 0;
            }
        }

        if (quantidadeDeLatasNaPista >= quantidadeMaximaDeLatasNaPista)
        {
            quantidadeDeLatasNaPista = 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeGeracao);
    }

    IEnumerator GerarUmaNovaLata()
    {
        Vector3 posicaoDeCriacao = AleatorizarPosicao();
        Collider[] colisores = Physics.OverlapSphere(posicaoDeCriacao, 1, LayerLata);

        while (colisores.Length > 0)
        {
            posicaoDeCriacao = AleatorizarPosicao();
            colisores = Physics.OverlapSphere(posicaoDeCriacao, 1, LayerLata);
            yield return null;
        }

        LatasCorrida latas = Instantiate(Latas, posicaoDeCriacao, transform.rotation).GetComponent<LatasCorrida>();
        latas.meuGerador = this;
        quantidadeDeLatasNaPista++;
    }

    IEnumerator GerarUmaUnicaLata()
    {
        LatasCorrida latas = Instantiate(Latas, transform.position, transform.rotation).GetComponent<LatasCorrida>();
        latas.meuGerador = this;

        // Configura a lata para ser única
        latas.eLataUnica = true;

        // Ajusta a posição da lata para diminuir o eixo Y
        Vector3 novaPosicao = latas.transform.position;
        novaPosicao.y = 0f; // Ajuste o valor para a altura desejada
        latas.transform.position = novaPosicao;
        yield return null;
    }

    Vector3 AleatorizarPosicao()
    {
        Vector3 posicao = Random.insideUnitSphere * distanciaDeGeracao;
        posicao += transform.position;
        posicao.y = 0.5f;

        return posicao;
    }

    public void DiminuirQuantidadeDeLatasNaPista()
    {
        quantidadeDeLatasNaPista--;
    }
}
