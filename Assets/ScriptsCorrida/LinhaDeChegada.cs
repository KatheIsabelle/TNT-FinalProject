using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinhaDeChegada : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerControllerCorridaCaminhoEnergizado>().CruzarLinhaDeChegada();
        }
    }
}
