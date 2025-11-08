using UnityEngine;

public class BatteryPickup : MonoBehaviour 
{
    public float batteryAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FlashlightToggle flash = other.GetComponentInChildren<FlashlightToggle>();

            if (flash != null)
            {
                flash.AddBattery(batteryAmount);
            }

            Destroy(gameObject);
        }
    }
}
