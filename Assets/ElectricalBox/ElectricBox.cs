using UnityEngine;
using System.Collections;

public class ElectricBox : MonoBehaviour
{
    [Header("Break Settings")]
    public float minBreakTime = 20f;
    public float maxBreakTime = 60f;
    public bool isFixed = true;

    [Header("Effects")]
    public ParticleSystem sparkEffect;
    public AudioClip breakSoundClip; // drag break.mp3 here

    private Light[] allLights;

    void Start()
    {
        // Get all spot lights in the scene
        allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        // Start random break coroutine
        StartCoroutine(BreakRoutine());
    }

    IEnumerator BreakRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBreakTime, maxBreakTime);
            yield return new WaitForSeconds(waitTime);
            BreakBox();
        }
    }

    void BreakBox()
    {
        if (isFixed)
        {
            isFixed = false;
            Debug.Log("⚡ Electric box has broken! Power outage!");

            // Turn off all spotlights
            foreach (Light l in allLights)
            {
                if (l.type == LightType.Spot)
                    l.enabled = false;
            }

            // Play spark effect
            if (sparkEffect != null)
                sparkEffect.Play();

            // Play break sound at box position
            if (breakSoundClip != null)
                AudioSource.PlayClipAtPoint(breakSoundClip, transform.position);
        }
    }

    // Call this from your HoldToFix script when player fixes the box
    public void FixBox()
    {
        if (!isFixed)
        {
            isFixed = true;
            Debug.Log("✅ Power restored!");

            foreach (Light l in allLights)
            {
                if (l.type == LightType.Spot)
                    l.enabled = true;
            }
        }
    }
}
