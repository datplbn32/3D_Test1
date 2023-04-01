using System;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
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
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 4f, ignoreRaycast);
        kartTransform.up = Vector3.Lerp(kartTransform.up, hitNear.normal, Time.fixedDeltaTime * 8);
        kartTransform.Rotate(0, currentRotation, 0);

        currentRotation += maxRotateSpeed * Time.deltaTime * horizontalInput;
        if (currentRotation > 180)
        {
            currentRotation -= 360;
        }
        else if (currentRotation < -180)
        {
            currentRotation += 360;
        }

        rb.AddForce(kartTransform.right * ((rb.velocity.magnitude) * horizontalInput),
            ForceMode.Acceleration);

        rb.AddForce(kartTransform.forward * ((acceleration + rb.velocity.magnitude) * verticalInput),
            ForceMode.Acceleration);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}