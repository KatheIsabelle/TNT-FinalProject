using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerCorrida : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Para movimentação lateral
        Vector3 movement = new Vector3(horizontal, 0.1f, speed) * Time.deltaTime;
        transform.Translate(movement);
    }
}
