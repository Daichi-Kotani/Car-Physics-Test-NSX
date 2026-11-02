using System.Collections.Generic;
using UnityEngine;

public class Tyre : MonoBehaviour
{
    // Serialized Fields
    [Header("Suspension")]
    [SerializeField] private float suspensionBaseDisplacement;
    [SerializeField] private float suspensionSpringCoefficient;
    [SerializeField] private float suspensionDampingCoefficient;
    [SerializeField] private float suspensionMotionRatio;

    [Header("Physics")]
    [SerializeField] private float radius;
    [SerializeField] private Transform forceApplicationPoint;
    [SerializeField] private float rayStartPoint;
    [SerializeField] private float rayMaxLength;
    [SerializeField] private List<Transform> translatingObjects;
    [SerializeField] private List<Transform> rotatingObjects;

    [Header("Normal Friction")]
    [SerializeField] private float externalForceCurveAmount;
    [SerializeField] private float externalForcePeakPoint;

    [Header("Lateral Slip")]
    [SerializeField] private float slipAnglePeakPoint; // Anything past this will decrease, anything before is linear based on cornering stiffness * slip angle in degrees
    [SerializeField] private float corneringStiffness;

    // by the grip / friction or something circle
    // define a maximum limit of grip as an ellipse based off of maximum lateral and longitudinal forces
    // the vector of both attempted grips in x and z will be combined and compared to the circle, with the magnitude being capped at the circle's level.
    // define the circle by taking the peak acceleration and peak braking force as the top and botto mextremes
    // then ues the maximum lateral force of the tyres as the left and right extremes.

    // Variables
    public bool debug;

    public float steering;

    private Vector3 surfaceNormal;
    private Vector3 currentVelocity;

    private bool grounded;

    private float suspensionDisplacement;
    private float suspensionVelocity;

    private float normalFriction;

    private Vector3 debugVector = new();

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
        HandleSlip();
        UpdateModels();
    }

    // Methods
    private void HandleSuspension()
    {
        // Raycast
        RaycastHit hit;
        grounded = Physics.Raycast(transform.position + transform.up * (suspensionBaseDisplacement + rayStartPoint), -transform.up, out hit, rayMaxLength);
        
        float initialDisplacement = suspensionDisplacement;

        if (grounded)
            suspensionDisplacement = (rayStartPoint - hit.distance + radius) * suspensionMotionRatio;

        suspensionVelocity = (suspensionDisplacement - initialDisplacement) / Time.fixedDeltaTime;

        float springForce = -suspensionSpringCoefficient * suspensionDisplacement - suspensionDampingCoefficient * suspensionVelocity;

        surfaceNormal = hit.normal;

        Vector3 springVector = surfaceNormal * -springForce;

        if (grounded && springForce < 0)
        {
            thisRigidbody.AddForceAtPosition(springVector, forceApplicationPoint.position, ForceMode.Force);
        }
        else
        {
            // suspensionDisplacement += springForce
        }

        normalFriction = 1f - Mathf.Pow(externalForceCurveAmount, springForce / externalForcePeakPoint);
    }

    private void HandleSlip()
    {
        if (grounded)
        {
            currentVelocity = thisRigidbody.GetPointVelocity(forceApplicationPoint.position);

            float lateralVelocity = Vector3.Dot(forceApplicationPoint.right, currentVelocity);
            float longitudinalVelocity = Vector3.Dot(forceApplicationPoint.forward, currentVelocity);

            float slipAngle = -Mathf.Atan2(lateralVelocity, longitudinalVelocity) * Mathf.Rad2Deg + steering;

            while (slipAngle > 180f)
            {
                slipAngle -= 360f;
            }

            while (slipAngle < -180f)
            {
                slipAngle += 360f;
            }

            float maxLateralGrip = slipAngle;

            if (Mathf.Abs(slipAngle) < slipAnglePeakPoint)
            {
                maxLateralGrip *= corneringStiffness;
            }
            else
            {
                maxLateralGrip = Mathf.Sign(slipAngle) * corneringStiffness * slipAnglePeakPoint;
            }

            float lateralGrip = Mathf.Clamp(lateralVelocity, -maxLateralGrip, maxLateralGrip);

            Vector3 lateralForce = forceApplicationPoint.right * lateralGrip;

            Vector3 lateralNormal = lateralForce - Vector3.Scale(surfaceNormal, lateralForce);

            debugVector = forceApplicationPoint.right * Mathf.Cos((slipAngle - 90) * Mathf.Deg2Rad) * 10f;

            thisRigidbody.AddForceAtPosition(lateralNormal, forceApplicationPoint.position, ForceMode.Force);

            Debug.Log(slipAngle);
        }
    }

    private void UpdateModels()
    {
        foreach (Transform i in translatingObjects)
        {
            i.localPosition = new(0f, suspensionDisplacement + suspensionBaseDisplacement, 0f);
        }

        foreach (Transform i in rotatingObjects)
        {
            i.localEulerAngles = new(0f, steering, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(forceApplicationPoint.position, forceApplicationPoint.position + debugVector);
    }
}
