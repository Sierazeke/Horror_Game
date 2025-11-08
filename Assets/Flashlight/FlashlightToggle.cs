using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light spotLight;
    public KeyCode toggleKey = KeyCode.F;

    private void Start()
    {
        if (spotLight == null) 
            spotLight = GetComponent<Light>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            spotLight.enabled = !spotLight.enabled;
        }
    }
}