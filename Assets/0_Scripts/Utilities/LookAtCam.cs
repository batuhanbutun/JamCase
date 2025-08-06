using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Transform camTransform;
    [SerializeField] private bool lookAtX = false;
    [SerializeField] private bool lookAtY = true;
    [SerializeField] private bool lookAtZ = true;

    private void Start()
    {
        camTransform = Camera.main.transform;
    }
    private void Update()
    {
        var direction = (transform.position - camTransform.position).normalized;
        if (!lookAtX)
        {
            direction.x = 0f;
        }
        if (!lookAtY)
        {
            direction.y = 0f;
        }
        if (!lookAtZ)
        {
            direction.z = 0f;
        }
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
