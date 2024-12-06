using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerCorrida : MonoBehaviour
{
    public Transform pontoA;
    public Transform pontoB;
    private Vector3 direcao;
    public float speed = 5f;
    private bool finalizarCamera = false;

    private void Start()
    {
        direcao = (pontoB.position - pontoA.position).normalized;
    }

    void Update()
    {
        if (finalizarCamera)
        {
            return; // Impede movimenta��o ap�s a corrida

        } 
        // Movimenta a c�mera em dire��o ao ponto B
        transform.position += direcao * speed * Time.deltaTime;

        // Verifica se a c�mera chegou ao ponto B
        if (Vector3.Distance(transform.position, pontoB.position) < 0.1f)
        {
            PararCameraNoPontoB();
        }
    }

    private void PararCameraNoPontoB()
    {
        finalizarCamera = true; // Desativa a movimenta��o
        transform.position = pontoB.position; // Garante que a posi��o seja exata
    }

    public void PosicionarCameraFinal()
    {
        finalizarCamera = true;
        transform.position = pontoB.position;
        transform.rotation = pontoB.rotation;
    }

}
