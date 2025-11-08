using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 4f;
    public KeyCode pickupKey = KeyCode.E;
    public LayerMask pickupMask;

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }
    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
        {
            IPickupable pickup = hit.collider.GetComponent<IPickupable>();

            if (pickup != null)
            {
                pickup.OnPickup();
            }
        }
    }
}
