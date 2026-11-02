using UnityEngine;

public class Display : NSX
{
    [SerializeField] private Transform lightObject;

    float amount = 0;

    private void Update()
    {
        ToggleLights();

        float x = throttleInput - brakeInput;

        amount += x * Time.deltaTime * 180;

        lightObject.localEulerAngles = new(50, amount, 0f);
        transform.Rotate(new(0f, -steeringInput * Time.deltaTime * 180, 0f));
    }
}
