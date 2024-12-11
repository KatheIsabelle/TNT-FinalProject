using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Script responsável por implementar a lógica de plataformas que são
desativadas e ativadas ao serem tocadas pelo jogador. Quando o jogador
toca na plataforma, ela some por um tempo e depois volta a aparecer.
*/

public class EffectPlataform : MonoBehaviour
{
    [Header("Var Desativação/Ativacao")]
    private bool isDisabled; 
    public float disableWait = 2f;
    public float enableWait = 0.5f;  

   [Header("Var posicao plataforma")]
    private Vector3 initialPosition; 
    private Quaternion initialRotation; 

    [Header("Var visualizao plataformas")]
    private Renderer platformRenderer; // Controle visual
    private Collider platformCollider; // Controle físico

    // Lista de tags com players
    public List<string> validTags = new List<string> { "Player", "Player2", "Player3", "Player4" };

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isDisabled && validTags.Contains(other.gameObject.tag))
        {
            StartCoroutine(enablePlatform());
            StartCoroutine(disablePlatform());
            //Debug.Log("Plataformas colididas: " + platformCount);
        }
    }

    


    private IEnumerator enablePlatform()
    {   

        yield return new WaitForSeconds(enableWait);

        // Reativa a plataforma
        if (platformRenderer != null)
        {
            platformRenderer.enabled = true; // Mostra a plataforma
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = true; // Reativa colisões
        }

        isDisabled = false;
  
    }


    private IEnumerator disablePlatform()
    {
        isDisabled = true;

        yield return new WaitForSeconds(disableWait);

        // desativa a plataforma
        if (platformRenderer != null)
        {
            platformRenderer.enabled = false; // remove visualização da plataforma
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = false; // desativa colisões
        }

    }
}
