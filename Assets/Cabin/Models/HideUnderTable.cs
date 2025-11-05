using UnityEngine;
using UnityEngine.InputSystem;

public class HideUnderTable : MonoBehaviour
{
    public Key hideKey = Key.E;
    public MonoBehaviour playerMovementScript;
    public float yOffset = 0.0f;

    private bool canHide = false;
    private bool isHiding = false;
    private Transform currentHidePoint;

    private Vector3 storedPosition;
    private Quaternion storedRotation;

    private CharacterController controller;
    private Collider playerCollider;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        isHiding = false;
        canHide = false;
    }

    void Update()
    {
        if (canHide && Keyboard.current[hideKey].wasPressedThisFrame)
        {
            if (!isHiding)
                StartHiding();
            else
                StopHiding();
        }
    }

    void StartHiding()
    {
        if (currentHidePoint == null) return;

        storedPosition = transform.position;
        storedRotation = transform.rotation;

        if (controller != null) controller.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;

        transform.position = currentHidePoint.position + Vector3.up * yOffset;
        transform.rotation = currentHidePoint.rotation;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        isHiding = true;
    }

    void StopHiding()
    {
        transform.position = storedPosition;
        transform.rotation = storedRotation;

        if (controller != null) controller.enabled = true;
        if (playerCollider != null) playerCollider.enabled = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        isHiding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TableHideSpot"))
        {
            Transform hidePoint = other.transform.Find("HidePoint");
            if (hidePoint != null)
            {
                currentHidePoint = hidePoint;
                canHide = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TableHideSpot"))
        {
            canHide = false;
            currentHidePoint = null;
        }
    }

    private void OnGUI()
    {
        if (canHide && !isHiding)
            GUI.Label(new Rect(10, 10, 300, 30), "Press E to hide under table");
        else if (isHiding)
            GUI.Label(new Rect(10, 10, 300, 30), "Press E to come out");
    }
}
