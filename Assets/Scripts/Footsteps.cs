using UnityEngine;
using System.Collections;

public class MovementSoundController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioSource audioSource;

    [Header("Movement Settings")]
    public float walkSpeedThreshold = 3.0f;
    public float runSpeedThreshold = 6.0f;

    [Header("Player Reference")]
    public CharacterController characterController;

    private bool wasGrounded = true;

    // Make enum public
    public enum MovementState
    {
        Idle,
        Walking,
        Running
    }

    // Expose currentState via public getter
    private MovementState currentState = MovementState.Idle;
    public MovementState CurrentState { get { return currentState; } }

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f;

        wasGrounded = characterController != null ? characterController.isGrounded : true;
    }

    void Update()
    {
        CheckMovementState();
        HandleAudio();
        CheckJumpAndLand();
    }

    void CheckMovementState()
    {
        if (characterController != null)
        {
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

        currentState = newState;
    }

    void HandleAudio()
    {
        switch (currentState)
        {
            case MovementState.Walking:
                if (!audioSource.isPlaying || audioSource.clip != walkingSound)
                {
                    audioSource.clip = walkingSound;
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
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

    void CheckJumpAndLand()
    {
        if (characterController != null)
        {
            bool isGrounded = characterController.isGrounded;

            if (!wasGrounded && isGrounded)
                PlayLandSound();

            if (wasGrounded && !isGrounded && characterController.velocity.y > 0.1f)
                PlayJumpSound();

            wasGrounded = isGrounded;
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
                PlayJumpSound();
        }
    }

    void PlayJumpSound()
    {
        if (jumpSound != null)
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
    }

    void PlayLandSound()
    {
        if (landSound != null)
            AudioSource.PlayClipAtPoint(landSound, transform.position);
    }
}
