using System;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    private float currentSpeed;
    [SerializeField] private float maxRotateSpeed;
    private float verticalInput;
    private float horizontalInput;
    private float currentRotation;
    [SerializeField] private float acceleration;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform kartTransform;
    [SerializeField] private LayerMask ignoreRaycast;
    private Vector3 offset;

    private void Awake()
    {
        offset = kartTransform.position - transform.position;
    }

    private void Update()
    {
        kartTransform.transform.position = transform.position + offset;
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        RaycastHit hitNear;
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 2.8f, ignoreRaycast);
        kartTransform.up = Vector3.Lerp(kartTransform.up, hitNear.normal, Time.fixedDeltaTime * 8);
        kartTransform.Rotate(0, currentRotation, 0);
        if (hitNear.collider)
        {
            currentRotation += maxRotateSpeed * Time.deltaTime * horizontalInput;
            if (currentRotation > 180)
            {
                currentRotation -= 360;
            }
            else if (currentRotation < -180)
            {
                currentRotation += 360;
            }
            
            rb.velocity = new Vector3(kartTransform.forward.x, 0, kartTransform.forward.z) * currentSpeed;
        }
        float mutiple = 1;
        if (verticalInput != 0)
        {
            if (verticalInput > 0 && currentSpeed < 0 || verticalInput < 0 && currentSpeed > 0)
            {
                mutiple = 2;
            }

            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed,
                acceleration * mutiple * Time.fixedDeltaTime * verticalInput);
        }
        else
        {
            mutiple = 0.98f;
            currentSpeed = rb.velocity.magnitude * mutiple * (currentSpeed > 0 ? 1 : -1);
        }
    }
}