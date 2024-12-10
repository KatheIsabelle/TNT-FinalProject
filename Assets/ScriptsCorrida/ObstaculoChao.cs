using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoChao : MonoBehaviour
{
    public float alturaMaxima = 5f; // Altura m�xima que o objeto ir� atingir
    public float velocidade = 2f;  // Velocidade do movimento
    public float tempoParado = 2f; // Tempo que o objeto ficar� parado no topo
    public float latasPerdidas = 5f;

    private Vector3 posicaoInicial;

    void Start()
    {
        // Armazena a posi��o inicial do objeto
        posicaoInicial = transform.position;

        // Inicia o movimento
        StartCoroutine(MoverObjeto());

    }


    IEnumerator MoverObjeto()
    {
        while (true) // Loop infinito
        {
            // Subir at� a altura m�xima
            while (transform.position.y < posicaoInicial.y + alturaMaxima)
            {
                transform.position += Vector3.up * velocidade * Time.deltaTime;
                yield return null;
            }

            // Garante que o objeto pare exatamente na altura m�xima
            transform.position = new Vector3(transform.position.x, posicaoInicial.y + alturaMaxima, transform.position.z);

            // Espera o tempo parado no topo
            yield return new WaitForSeconds(tempoParado);

            // Descer at� a posi��o inicial
            while (transform.position.y > posicaoInicial.y)
            {
                transform.position -= Vector3.up * velocidade * Time.deltaTime;
                yield return null;
            }

            // Garante que o objeto volte exatamente � posi��o inicial
            transform.position = posicaoInicial;

            // Espera o tempo parado na base
            yield return new WaitForSeconds(tempoParado);
        }
    }

    void OnTriggerEnter(Collider objetoDeColisao)
    {
        if (objetoDeColisao.tag == "Player")
        {
            objetoDeColisao.GetComponent<PlayerControllerCorrida>().PerdeLatas(latasPerdidas);
        }
    }
}
