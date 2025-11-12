using UnityEngine;
using UnityEngine.UI;

public class HoldToFix : MonoBehaviour
{
    public float fixRange = 3f;
    public KeyCode fixKey = KeyCode.E;
    public float holdTime = 3f;
    private float holdProgress = 0f;
    private ElectricBox currentBox;

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fixRange))
        {
            ElectricBox box = hit.collider.GetComponent<ElectricBox>();
            if (box != null && !box.isFixed)
            {
                currentBox = box;

                if (Input.GetKey(fixKey))
                {
                    holdProgress += Time.deltaTime;
                    if (holdProgress >= holdTime)
                    {
                        currentBox.FixBox();
                        holdProgress = 0f;
                    }
                }
                else if (Input.GetKeyUp(fixKey))
                {
                    holdProgress = 0f; // reset if player lets go
                }
            }
            else
            {
                holdProgress = 0f;
                currentBox = null;
            }
        }
        else
        {
            holdProgress = 0f;
            currentBox = null;
        }
    }
}