using UnityEngine;
using UnityEngine.InputSystem;

public class NSX : MonoBehaviour
{
    // Serialized Fields
    [Header("Tyres")]
    [SerializeField] private Tyre tyreFl;
    [SerializeField] private Tyre tyreFR;
    [SerializeField] private Tyre tyreBl;
    [SerializeField] private Tyre tyreBR;

    [Header("Popups")]
    [SerializeField] private Transform popups;
    [SerializeField] private float popupUpPosition;
    [SerializeField] private float popupDownPosition;
    [SerializeField] private float popupSpeed;

    // Variables
    private float popupPosition;

    // References
    private Rigidbody thisRigidbody;

    // Unity Messages
    private void Awake()
    {
        thisRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        ToggleLights();
    }

    // Methods
    private void ToggleLights()
    {
        float speed = popupSpeed
             * (lightInput
             ? (popupPosition < popupDownPosition ? 0f : -1f)
             : (popupPosition > popupUpPosition ? 0f : 1f));

        if (popupPosition < popupDownPosition)
            popupPosition = popupDownPosition;

        if (popupPosition > popupUpPosition)
            popupPosition = popupUpPosition;

        popupPosition += speed * Time.deltaTime;

        popups.localRotation = Quaternion.Euler(popupPosition, 0f, 0f);
    }

    // Input
    private bool lightInput = false;
    private float throttleInput;
    private float brakeInput;
    private float steeringInput;

    public void GetLightToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lightInput = !lightInput;
        }
    }

    public void GetThrottleInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            throttleInput = context.ReadValue<float>();
        }
        else
        {
            throttleInput = 0f;
        }
    }

    public void GetBrakeInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            brakeInput = context.ReadValue<float>();
        }
        else
        {
            brakeInput = 0f;
        }
    }

    public void GetSteeringInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            steeringInput = context.ReadValue<float>();
        }
        else
        {
            steeringInput = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        if (thisRigidbody == null)
            return;

        Gizmos.DrawLine(transform.position, transform.position - transform.up * thisRigidbody.mass * 9.81f);
    }
}
