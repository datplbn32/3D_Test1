using System;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    public Transform cameraPosition;
    public List<ParticleSystem> listParticles;
    public List<TrailRenderer> ListTrailRenderers;

    private float currentMaxSpeed;
    [SerializeField] float minCanDriftSpeed = 15;
    [SerializeField] private float maxSpeed = 50;
    [SerializeField] private float maxDriftSpeed = 30;
    
    [SerializeField] private float maxRotateSpeed;
    [SerializeField] private float maxDriftRotateSpeed;
    private float currentMaxRotateSpeed;
    
    private float verticalInput;
    private float horizontalInput;
    private float currentRotation;
    [SerializeField] private float acceleration;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform kartTransform;
    [SerializeField] private LayerMask ignoreRaycast;
    private Vector3 offset;
    private bool inputDrift = false;
    private bool drifting = false;

    private void Awake()
    {
        offset = kartTransform.position - transform.position;
    }

    private void Update()
    {
        kartTransform.transform.position = transform.position + offset;
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
        inputDrift = Input.GetKey(KeyCode.Space);
        if (inputDrift && rb.velocity.magnitude >= minCanDriftSpeed)
        {
            currentMaxRotateSpeed = Mathf.Lerp(currentMaxRotateSpeed, maxDriftRotateSpeed, 0.05f);
            currentMaxSpeed = maxDriftSpeed;
        }
        else
        {
            currentMaxRotateSpeed = maxRotateSpeed;
            currentMaxSpeed = maxSpeed;
        }
    }

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out var hitNear, 4f, ignoreRaycast);
        kartTransform.up = Vector3.Lerp(kartTransform.up, hitNear.normal, Time.fixedDeltaTime * 8);
        kartTransform.Rotate(0, currentRotation, 0);

        currentRotation += currentMaxRotateSpeed * Time.deltaTime * horizontalInput;
        if (currentRotation > 180)
        {
            currentRotation -= 360;
        }
        else if (currentRotation < -180)
        {
            currentRotation += 360;
        }
        
        if (hitNear.collider && inputDrift && horizontalInput!=0 && verticalInput > 0)
        {
            drifting = true;
        }
        else
        {
            drifting = false;
        }
        
        if (drifting)
        {
            // rb.AddForce(kartTransform.forward * (acceleration * verticalInput),
            //     ForceMode.Acceleration);
            // rb.AddForce(-kartTransform.right * (acceleration * verticalInput * horizontalInput),
            //     ForceMode.Acceleration);
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            for (int i = 0; i < listParticles.Count; i++)
            {
                if (!listParticles[i].isPlaying)
                {
                    listParticles[i].Play();
                }
            }
            for (int i = 0; i < ListTrailRenderers.Count; i++)
            {
                ListTrailRenderers[i].gameObject.SetActive(true);
            }
        }
        else
        {
            rb.drag = 2f;
            rb.angularDrag = 2f;
            for (int i = 0; i < listParticles.Count; i++)
            {
                if (listParticles[i].isPlaying)
                {
                    listParticles[i].Pause();
                }
            }
            for (int i = 0; i < ListTrailRenderers.Count; i++)
            {
                ListTrailRenderers[i].gameObject.SetActive(false);
            }
        }

        rb.AddForce(kartTransform.right * ((rb.velocity.magnitude) * horizontalInput),
            ForceMode.Acceleration);

        rb.AddForce(kartTransform.forward * ((acceleration + rb.velocity.magnitude) * verticalInput),
            ForceMode.Acceleration);

        if (rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.normalized * currentMaxSpeed, 0.1f);
        }
    }
}