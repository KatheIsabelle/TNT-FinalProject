using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScriptRingDing : MonoBehaviour
{
    public float pushForce = 30f;

    private void OnTriggerEnter(Collider collider) {

        Debug.Log(collider.name);
        // Verifica se o objeto colidido tem o componente Rigidbody
        Rigidbody otherRigidbody = collider.GetComponent<Rigidbody>();
        if (otherRigidbody != null && collider.gameObject.CompareTag("Player")) {
            // Calcula a dire��o do empurr�o
            Vector3 pushDirection = (collider.transform.position - transform.position).normalized;

            // Aplica a for�a no outro jogador
            otherRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            
        }
    }
}
