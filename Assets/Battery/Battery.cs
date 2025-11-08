using UnityEngine;

public class Battery : MonoBehaviour, IPickupable
{
    public void OnPickup()
    {
        Debug.Log("Battery picked up!");
        Destroy(gameObject); 
    }
}
    