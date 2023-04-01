using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform CameraPos;
    public Transform LookAtPoint;
    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, CameraPos.position, 10f * (CameraPos.position - transform.position).magnitude * Time.deltaTime);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(LookAtPoint.position - transform.position, Vector3.up), 180 * Time.deltaTime);
        transform.rotation = rotation;
    }
}
