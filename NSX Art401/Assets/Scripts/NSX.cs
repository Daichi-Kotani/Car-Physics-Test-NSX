using UnityEngine;
using UnityEngine.InputSystem;

public class NSX : MonoBehaviour
{
    // Serialized Fields
    [Header("Tyres")]
    [SerializeField] private Tyre tyreFl;
    [SerializeField] private Tyre tyreFr;
    [SerializeField] private Tyre tyreBl;
    [SerializeField] private Tyre tyreBr;

    [Header("Popups")]
    [SerializeField] private Transform popups;
    [SerializeField] private float popupUpPosition;
    [SerializeField] private float popupDownPosition;
    [SerializeField] private float popupSpeed;

    // Variables
    private float popupPosition;

    // References
    private Rigidbody thisRigidbody;

    // Input
    public bool lightInput = false;
    public float throttleInput;
    public float brakeInput;
    public float steeringInput;

    // Unity Messages
    private void Awake()
    {
        thisRigidbody = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        ToggleLights();
        UpdateSteering();

        thisRigidbody.AddRelativeForce(new(0f, 0f, 1000f * throttleInput));
        thisRigidbody.AddRelativeForce(new(0f, 0f, -4000f * brakeInput));
    }

    // Methods
    public void ToggleLights()
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

    private void UpdateSteering()
    {
        tyreFl.steering = steeringInput * 20;
        tyreFr.steering = steeringInput * 20;
    }

    // Input
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
}
