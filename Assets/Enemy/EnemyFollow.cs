using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;

    [Header("Map Borders")]
    public Vector3 minBounds; // bottom-left-back corner
    public Vector3 maxBounds; // top-right-front corner

    [Header("Hearing & Sight")]
    public float visionRange = 3f;        // half-blind
    public float hearingRange = 8f;       // hears running/jumping
    public MovementSoundController playerSounds;

    [Header("Light Fear")]
    public LayerMask lightMask; // flashlight + house lights
    private bool isInLight = false;

    private Vector3 roamTarget;

    void Start()
    {
        PickNewRoamTarget();
    }

    void Update()
    {
        if (player == null) return;

        // Check if in light
        isInLight = Physics.CheckSphere(transform.position, 0.5f, lightMask);
        if (isInLight)
        {
            gameObject.SetActive(false); // disappear
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Half-blind vision
        if (distToPlayer <= visionRange)
        {
            MoveToward(player.position);
        }
        // Hearing: running/jumping
        else if ((playerSounds.CurrentState == MovementSoundController.MovementState.Running
                  || Input.GetButton("Jump")) && distToPlayer <= hearingRange)
        {
            MoveToward(player.position);
        }
        else
        {
            // Aimless roaming
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
        transform.forward = direction;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void PickNewRoamTarget()
    {
        roamTarget = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            transform.position.y, // keep same height
            Random.Range(minBounds.z, maxBounds.z)
        );
    }

    void OnDrawGizmosSelected()
    {
        // Vision/hearing
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        // Map borders
        Gizmos.color = Color.green;
        Vector3 center = (minBounds + maxBounds) / 2f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}
