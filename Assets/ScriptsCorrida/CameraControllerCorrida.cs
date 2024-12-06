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
            return; // Impede movimentação após a corrida

        } 
        // Movimenta a câmera em direção ao ponto B
        transform.position += direcao * speed * Time.deltaTime;

        // Verifica se a câmera chegou ao ponto B
        if (Vector3.Distance(transform.position, pontoB.position) < 0.1f)
        {
            PararCameraNoPontoB();
        }
    }

    private void PararCameraNoPontoB()
    {
        finalizarCamera = true; // Desativa a movimentação
        transform.position = pontoB.position; // Garante que a posição seja exata
    }

    public void PosicionarCameraFinal()
    {
        finalizarCamera = true;
        transform.position = pontoB.position;
        transform.rotation = pontoB.rotation;
    }

}
