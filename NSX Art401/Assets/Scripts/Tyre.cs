using System.Collections.Generic;
using UnityEngine;

public class Tyre : MonoBehaviour
{
    // Serialized Fields
    [Header("Suspension")]
    [SerializeField] private float suspensionBaseDisplacement;
    [SerializeField] private float suspensionSpringCoefficient;
    [SerializeField] private float suspensionDampingCoefficient;

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
        UpdateModels();
    }

    // Methods
    private void HandleSuspension()
    {
        // Raycast
        RaycastHit hit;
        bool grounded = Physics.Raycast(transform.position + transform.up * (suspensionBaseDisplacement + rayStartPoint), -transform.up, out hit, rayMaxLength);
        
        float initialDisplacement = suspensionDisplacement;

        if (grounded)
            suspensionDisplacement = rayStartPoint - hit.distance + radius;

        suspensionVelocity = (suspensionDisplacement - initialDisplacement) / Time.fixedDeltaTime;

        float springForce = -suspensionSpringCoefficient * suspensionDisplacement - suspensionDampingCoefficient * suspensionVelocity;

        Vector3 springVector = hit.normal * -springForce;

        if (grounded && springForce < 0)
        {
            thisRigidbody.AddForceAtPosition(springVector, forceApplicationPoint.position, ForceMode.Force);
        }
        else
        {
            // suspensionDisplacement += springForce
        }
    }

    private void HandleSlip()
    {
        // Test assuming longitudinal slip  is 0%
    }

    private void UpdateModels()
    {
        foreach (Transform i in translatingObjects)
        {
            i.localPosition = new(0f, suspensionDisplacement + suspensionBaseDisplacement, 0f);
        }
    }
}
