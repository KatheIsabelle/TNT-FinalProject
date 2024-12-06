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

    void Start()
    {
        jogador = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        bool possoGerarLatasDistancia = Vector3.Distance(transform.position, jogador.transform.position) <= distanciaDoJogadorParaGeracao;

        if (possoGerarLatasDistancia && quantidadeDeLatasNaPista < quantidadeMaximaDeLatasNaPista)
        {
            contadorTempo += Time.deltaTime;

            if (contadorTempo >= tempoGeradorLatas)
            {
                StartCoroutine(GerarUmNovaLata());
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

    IEnumerator GerarUmNovaLata()
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
