using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerCaminhoEnergizado : MonoBehaviour
{
    public Transform pontoA;
    public Transform pontoB;
    private Vector3 direcao;
    public float speed = 5f;
    private bool finalizarCamera = false;
    private bool moverCamera = false; // Controle de movimentação da câmera
    public float offsetDestruicao = 10f; // Distância atrás da câmera para destruir objetos
    public LayerMask objetosParaDestruir; // Define quais objetos podem ser destruídos

    private void Start()
    {
        direcao = (pontoB.position - pontoA.position).normalized;
    }

    void Update()
    {
        if (!moverCamera || finalizarCamera)
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

        // Destrói objetos atrás da câmera
        DestruirObjetosAposCamera();
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

    public void AtivarMovimentoCamera()
    {
        moverCamera = true; // Permite que a câmera comece a se mover
    }

    private void DestruirObjetosAposCamera()
    {
        // Define a posição da linha de destruição com base no eixo Z da câmera
        float limiteDestruicao = transform.position.z - offsetDestruicao;

        // Encontra todos os objetos com o layer desejado
        Collider[] objetos = Physics.OverlapBox(
            new Vector3(transform.position.x, transform.position.y, limiteDestruicao), // Centro do box
            new Vector3(100f, 100f, 0.1f), // Tamanho do box (ajuste se necessário)
            Quaternion.identity, // Rotação
            objetosParaDestruir // LayerMask
        );

        // Destrói todos os objetos encontrados
        foreach (Collider objeto in objetos)
        {
            Destroy(objeto.gameObject);
        }
    }

}
