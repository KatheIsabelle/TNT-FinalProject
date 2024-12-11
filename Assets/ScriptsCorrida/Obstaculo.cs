using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    public Transform pontoA;
    public Transform pontoB;
    public float velocidade = 5f;
    private Vector3 direcao;
    private float tempo = 0f;
    public float latasPerdidas = 3;

    void Start()
    {
        direcao = (pontoB.position - pontoA.position).normalized;
    }

    void Update()
    {
        transform.position += direcao * velocidade * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, pontoA.position.y, transform.position.z);
        tempo += Time.deltaTime * 2f;

        if (Vector2.Distance(transform.position, pontoB.position) < 0.5f)
        {
            direcao = (pontoA.position - pontoB.position).normalized;
        }
        else if (Vector2.Distance(transform.position, pontoA.position) < 0.5f)
        {
            direcao = (pontoB.position - pontoA.position).normalized;
        }
    }

    void OnTriggerEnter(Collider objetoDeColisao)
    {
        if (objetoDeColisao.tag == "Player")
        {
            objetoDeColisao.GetComponent<PlayerControllerCorridaCaminhoEnergizado>().PerdeLatas(latasPerdidas);
        }
    }
}
