using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;

    void Update()
    {
        if (player == null) return;

        // Move toward player
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        // Make the enemy face the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.forward = direction;
    }
}
