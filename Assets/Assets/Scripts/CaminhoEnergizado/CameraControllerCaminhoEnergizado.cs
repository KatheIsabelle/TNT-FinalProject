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
    private bool moverCamera = false; // Controle de movimenta��o da c�mera
    public float offsetDestruicao = 10f; // Dist�ncia atr�s da c�mera para destruir objetos
    public LayerMask objetosParaDestruir; // Define quais objetos podem ser destru�dos

    private void Start()
    {
        direcao = (pontoB.position - pontoA.position).normalized;
    }

    void Update()
    {
        if (!moverCamera || finalizarCamera)
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

        // Destr�i objetos atr�s da c�mera
        DestruirObjetosAposCamera();
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

    public void AtivarMovimentoCamera()
    {
        moverCamera = true; // Permite que a c�mera comece a se mover
    }

    private void DestruirObjetosAposCamera()
    {
        // Define a posi��o da linha de destrui��o com base no eixo Z da c�mera
        float limiteDestruicao = transform.position.z - offsetDestruicao;

        // Encontra todos os objetos com o layer desejado
        Collider[] objetos = Physics.OverlapBox(
            new Vector3(transform.position.x, transform.position.y, limiteDestruicao), // Centro do box
            new Vector3(100f, 100f, 0.1f), // Tamanho do box (ajuste se necess�rio)
            Quaternion.identity, // Rota��o
            objetosParaDestruir // LayerMask
        );

        // Destr�i todos os objetos encontrados
        foreach (Collider objeto in objetos)
        {
            Destroy(objeto.gameObject);
        }
    }

}
