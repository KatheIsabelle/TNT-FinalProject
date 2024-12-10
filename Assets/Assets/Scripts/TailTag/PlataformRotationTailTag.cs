using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotationTailTag : MonoBehaviour 
{
    public float rotationSpeed = 50f; // Velocidade de rota��o

    // Intensidade do empurr�o
    public float pushForce = 60f; // Ajuste conforme necess�rio

    void Update() 
    {
        // Rotaciona a plataforma no eixo Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

    }

    // M�todo para obter a dire��o da rota��o
    public Vector3 GetRotationDirection() 
    {
        return transform.right * Mathf.Sign(rotationSpeed); // Dire��o no eixo X
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();
            if (playerRb != null) {
                PushPlayer(collision, playerRb);
            }
        }
    }
    void PushPlayer(Collision collision, Rigidbody playerRb) {
        // Obt�m o ponto de contato mais pr�ximo do obst�culo
        Vector3 contactPoint = collision.contacts[0].point;

        // Calcula a dire��o do empurr�o com base no ponto de contato
        Vector3 pushDirection = (collision.collider.transform.position - contactPoint).normalized;
        pushDirection.y = 0; // Garante que n�o empurre para cima

        // Aplica for�a ao Rigidbody do jogador
        playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}