using UnityEngine;
using System.Collections;

public class MovementSoundController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioSource audioSource;

    [Header("Movement Settings")]
    public float walkSpeedThreshold = 3.0f;
    public float runSpeedThreshold = 6.0f;

    [Header("Player Reference")]
    public CharacterController characterController;
    // Alternatively, use Rigidbody:
    // public Rigidbody playerRigidbody;

    private bool isMoving = false;
    private MovementState currentState = MovementState.Idle;

    private enum MovementState
    {
        Idle,
        Walking,
        Running
    }

    void Start()
    {
        // If no audio source is assigned, try to get one
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // If no character controller is assigned, try to get one
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        // Configure audio source
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f; // 3D sound
    }

    void Update()
    {
        CheckMovementState();
        HandleAudio();
    }

    void CheckMovementState()
    {
        if (characterController != null)
        {
            // Using CharacterController velocity
            Vector3 horizontalVelocity = new Vector3(
                characterController.velocity.x,
                0,
                characterController.velocity.z
            );
            float currentSpeed = horizontalVelocity.magnitude;

            UpdateMovementState(currentSpeed);
        }
        else
        {
            // Alternative: Using input-based detection
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
            bool hasInput = inputDirection.magnitude > 0.1f;

            if (hasInput)
            {
                currentState = isSprinting ? MovementState.Running : MovementState.Walking;
            }
            else
            {
                currentState = MovementState.Idle;
            }
        }
    }

    void UpdateMovementState(float currentSpeed)
    {
        MovementState newState;

        if (currentSpeed < 0.1f)
        {
            newState = MovementState.Idle;
        }
        else if (currentSpeed < runSpeedThreshold)
        {
            newState = MovementState.Walking;
        }
        else
        {
            newState = MovementState.Running;
        }

        // Only update if state changed
        if (newState != currentState)
        {
            currentState = newState;
        }
    }

    void HandleAudio()
    {
        switch (currentState)
        {
            case MovementState.Walking:
                if (!audioSource.isPlaying || audioSource.clip != walkingSound)
                {
                    audioSource.clip = walkingSound;
                    audioSource.pitch = Random.Range(0.9f, 1.1f); // Slight variation
                    audioSource.volume = 0.7f;
                    audioSource.Play();
                }
                break;

            case MovementState.Running:
                if (!audioSource.isPlaying || audioSource.clip != runningSound)
                {
                    audioSource.clip = runningSound;
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.volume = 0.8f;
                    audioSource.Play();
                }
                break;

            case MovementState.Idle:
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                break;
        }
    }

    // Optional: Method to manually trigger sprinting
    public void SetSprinting(bool isSprinting)
    {
        if (isSprinting && currentState == MovementState.Walking)
        {
            currentState = MovementState.Running;
        }
        else if (!isSprinting && currentState == MovementState.Running)
        {
            currentState = MovementState.Walking;
        }
    }
}