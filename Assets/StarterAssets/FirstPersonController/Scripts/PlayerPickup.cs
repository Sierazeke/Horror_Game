using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public KeyCode pickupKey = KeyCode.E;
    public LayerMask pickupMask = ~0; // set to only the layer(s) you use for pickups (or leave as Everything)
    public Transform uiHintPoint; // optional: where to show UI hint

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("PlayerPickup: No Camera tagged MainCamera in scene.");
                return;
            }

            // Ray from screen center (works for FPS)
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.green, 1f);

            // IMPORTANT: include triggers so ray hits trigger colliders too
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask, QueryTriggerInteraction.Collide))
            {
                Battery battery = hit.collider.GetComponent<Battery>();
                if (battery != null)
                {
                    Debug.Log("PlayerPickup: Hit battery: " + hit.collider.name);
                    battery.OnPickup();
                }
                else
                {
                    // helpful debug
                    Debug.Log("PlayerPickup: Hit '" + hit.collider.name + "' but no Battery component found on it.");
                }
            }
            else
            {
                Debug.Log("PlayerPickup: nothing hit within range.");
            }
        }
    }
}
