using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotationTailTag : MonoBehaviour 
{
    public float rotationSpeed = 50f; // Velocidade de rotação

    // Intensidade do empurrão
    public float pushForce = 60f; // Ajuste conforme necessário

    void Update() 
    {
        // Rotaciona a plataforma no eixo Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

    }

    // Método para obter a direção da rotação
    public Vector3 GetRotationDirection() 
    {
        return transform.right * Mathf.Sign(rotationSpeed); // Direção no eixo X
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
        // Obtém o ponto de contato mais próximo do obstáculo
        Vector3 contactPoint = collision.contacts[0].point;

        // Calcula a direção do empurrão com base no ponto de contato
        Vector3 pushDirection = (collision.collider.transform.position - contactPoint).normalized;
        pushDirection.y = 0; // Garante que não empurre para cima

        // Aplica força ao Rigidbody do jogador
        playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}