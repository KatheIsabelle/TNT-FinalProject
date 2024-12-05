using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotation : MonoBehaviour 
{
    public float rotationSpeed = 50f; // Velocidade de rota��o

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
}