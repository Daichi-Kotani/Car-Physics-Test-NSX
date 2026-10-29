using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tyre : MonoBehaviour
{
    // Serialized Fields
    [Header("Suspension")]
    [SerializeField] private float suspensionBaseDisplacement;
    [SerializeField] private float suspensionSpringK;

    [Header("Physics")]
    [SerializeField] private float radius;
    [SerializeField] private Transform forceApplicationPoint;
    [SerializeField] private float rayStartPoint;
    [SerializeField] private float rayMaxLength;
    [SerializeField] private List<Transform> translatingObjects;
    [SerializeField] private List<Transform> rotatingObjects;

    // Variables

    private float suspensionDisplacement;
    private float suspensionVelocity;
    private float forceX;
    private float forceY;
    private float forceZ;

    // References
    private Rigidbody thisRigidbody;

    // Unity Messages
    private void Awake()
    {
        thisRigidbody = GetComponentInParent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        HandleSuspension();
        ApplyForces();
        UpdateModels();
    }

    // Methods
    private void HandleSuspension()
    {
        // Raycast
        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position + transform.up * rayStartPoint, -transform.up, out hit, rayMaxLength);

        if (!grounded)
        {
            forceY = 0;
            return;
        }

        suspensionDisplacement = rayStartPoint + suspensionBaseDisplacement - hit.distance + radius;

        float springForce = -suspensionSpringK * suspensionDisplacement;

        // Spring (o) | damping (x)
        forceY = -springForce;

        Debug.Log(hit.distance + " | " + springForce);
    }

    private void ApplyForces()
    {
        if (forceY > 0)
            thisRigidbody.AddForceAtPosition(transform.up * forceY, forceApplicationPoint.position, ForceMode.Force);
    }

    private void UpdateModels()
    {
        foreach (Transform i in translatingObjects)
        {
            i.localPosition = new(0f, suspensionDisplacement + suspensionBaseDisplacement, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (thisRigidbody == null)
            return;

        Gizmos.DrawLine(transform.position, transform.position + transform.up * suspensionDisplacement);
    }
}
