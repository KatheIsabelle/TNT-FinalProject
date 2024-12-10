using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTailTag : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] [Range(0.01f,1f)] private float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.one;

    private void LateUpdate() {
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }

}
