using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoChao : MonoBehaviour
{
    public float alturaMaxima = 5f; // Altura máxima que o objeto irá atingir
    public float velocidade = 2f;  // Velocidade do movimento
    public float tempoParado = 2f; // Tempo que o objeto ficará parado no topo
    public float latasPerdidas = 5f;

    private Vector3 posicaoInicial;

    void Start()
    {
        // Armazena a posição inicial do objeto
        posicaoInicial = transform.position;

        // Inicia o movimento
        StartCoroutine(MoverObjeto());

    }


    IEnumerator MoverObjeto()
    {
        while (true) // Loop infinito
        {
            // Subir até a altura máxima
            while (transform.position.y < posicaoInicial.y + alturaMaxima)
            {
                transform.position += Vector3.up * velocidade * Time.deltaTime;
                yield return null;
            }

            // Garante que o objeto pare exatamente na altura máxima
            transform.position = new Vector3(transform.position.x, posicaoInicial.y + alturaMaxima, transform.position.z);

            // Espera o tempo parado no topo
            yield return new WaitForSeconds(tempoParado);

            // Descer até a posição inicial
            while (transform.position.y > posicaoInicial.y)
            {
                transform.position -= Vector3.up * velocidade * Time.deltaTime;
                yield return null;
            }

            // Garante que o objeto volte exatamente à posição inicial
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
