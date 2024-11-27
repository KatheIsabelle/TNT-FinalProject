using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingPlatform : MonoBehaviour
{
    [Header("Var Vencer")]
    public static int platformCount;
    public GameObject winText;

    [Header("Var Plataforma")]
    public float fallWait = 2f;
    public float destroyWait = 1f;
    private bool isFalling;


    void Start()
    {
        // count start 0
        if (platformCount == 0)
        {
            platformCount = 0;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (!isFalling && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
            platformCount++;
            Debug.Log("Plataformas colididas: " + platformCount);

            CheckWin();
        }
    }


    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        yield return new WaitForSeconds(destroyWait);
        Destroy(gameObject);
    }


    void CheckWin()
    {
        if (platformCount >= 5) 
        {
            if (winText != null)
            {
                winText.SetActive(true);
            }
            Debug.Log("You Win!");
        }
    }
}
