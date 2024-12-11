using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMovimento : MonoBehaviour
{
    public Transform pontoA;
    public Transform pontoB;
    public float velocidade = 5f;
    private Vector3 direcao;
    private float tempo = 0f;
    void Start()
    {
        direcao = (pontoB.position - pontoA.position).normalized;
    }

    void Update()
    {
        transform.position += direcao * velocidade * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, pontoA.position.y, transform.position.z);
        tempo += Time.deltaTime * 2f;

        if (Vector2.Distance(transform.position, pontoB.position) < 0.1f)
        {
            transform.position = pontoA.position;
            tempo = 0f; 
        }
    }
}
