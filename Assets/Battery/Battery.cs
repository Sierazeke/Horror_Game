using UnityEngine;

public class Battery : MonoBehaviour
{
    public float batteryAmount = 25f;

    public void OnPickup()
    {
        // Use FindAnyObjectByType for speed — fine if you only have one FlashlightToggle.
        FlashlightToggle flash = Object.FindAnyObjectByType<FlashlightToggle>();
        if (flash != null)
        {
            flash.AddBattery(batteryAmount);
            Debug.Log($"Battery: gave {batteryAmount}% to flashlight.");
        }
        else
        {
            Debug.LogWarning("Battery: No FlashlightToggle found in scene.");
        }

        Destroy(gameObject);
    }
}
