using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Flashlight")]
    public Light spotLight;
    public KeyCode toggleKey = KeyCode.F;

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float drainRate = 5f;      
    public float rechargeRate = 20f;  
    public bool isRecharging = false;

    void Start()
    {
        if (spotLight == null)
            spotLight = GetComponent<Light>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && currentBattery > 0f)
        {
            spotLight.enabled = !spotLight.enabled;
        }

        if (spotLight.enabled && currentBattery > 0f)
        {
            currentBattery -= drainRate * Time.deltaTime;

            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                spotLight.enabled = false; // auto turn off like a sad flashlight
            }
        }

        if (isRecharging)
        {
            currentBattery += rechargeRate * Time.deltaTime;
            if (currentBattery > maxBattery)
                currentBattery = maxBattery;
        }
    }

    public void AddBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery)
            currentBattery = maxBattery;
    }
}
