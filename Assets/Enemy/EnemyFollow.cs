using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;

    [Header("Map Borders")]
    public Vector3 minBounds;
    public Vector3 maxBounds;

    [Header("Hearing & Sight")]
    public float visionRange = 3f;
    public float hearingRange = 8f;          // Running / jumping
    public float walkingHearingRange = 4f;   // NEW: hears walking but only super close
    public MovementSoundController playerSounds;

    [Header("Light Fear")]
    public LayerMask lightMask;
    private bool isInLight = false;

    private Vector3 roamTarget;

    void Start()
    {
        PickNewRoamTarget();
    }

    void Update()
    {
        if (player == null) return;

        // Light check
        isInLight = Physics.CheckSphere(transform.position, 0.5f, lightMask);
        if (isInLight)
        {
            gameObject.SetActive(false);
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Vision
        if (distToPlayer <= visionRange)
        {
            MoveToward(player.position);
        }
        // Hearing running/jumping
        else if ((playerSounds.CurrentState == MovementSoundController.MovementState.Running
                  || Input.GetButton("Jump")) && distToPlayer <= hearingRange)
        {
            MoveToward(player.position);
        }
        // NEW: Hearing walking (only close)
        else if (playerSounds.CurrentState == MovementSoundController.MovementState.Walking
                 && distToPlayer <= walkingHearingRange)
        {
            MoveToward(player.position);
        }
        else
        {
            // Roaming
            if (Vector3.Distance(transform.position, roamTarget) < 0.2f)
            {
                PickNewRoamTarget();
            }
            MoveToward(roamTarget);
        }
    }

    void MoveToward(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 0.8f))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                if (target != player.position)
                {
                    PickNewRoamTarget();
                }
                return;
            }
        }

        // No wall → move normally
        transform.forward = direction;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void PickNewRoamTarget()
    {
        roamTarget = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            transform.position.y,
            Random.Range(minBounds.z, maxBounds.z)
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, walkingHearingRange);

        Gizmos.color = Color.green;
        Vector3 center = (minBounds + maxBounds) / 2f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}
