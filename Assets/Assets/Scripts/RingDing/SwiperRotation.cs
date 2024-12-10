using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiperRotation : MonoBehaviour 
{
    [SerializeField] private float rotationSpeed = 50f; // Velocidade de rotação

    void Start() {
        transform.eulerAngles = new Vector3(0f, Random.Range(0f,360f),0f);
    }

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
}