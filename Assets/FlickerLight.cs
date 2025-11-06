using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public Light flickerLight;       // drag your light here
    public float minIntensity = 4f;
    public float maxIntensity = 6f;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();
        InvokeRepeating("Flicker", 0f, flickerSpeed);
    }

    void Flicker()
    {
        flickerLight.intensity = Random.Range(minIntensity, maxIntensity);
    }
}
