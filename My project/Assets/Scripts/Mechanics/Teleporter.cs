using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    public Transform destination; // Set this in the Inspector to the partner gate.
    public float cooldownTime = 2f; // Cooldown time in seconds.
    public Vector3 teleportOffset = new Vector3(1f, 0, 0); // Adjust the offset as needed.

    private bool isOnCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOnCooldown)
        {
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        isOnCooldown = true; // Start cooldown
        player.position = destination.position + teleportOffset; // Move player with offset

        // Wait for cooldown at this gate
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false; // End cooldown
    }
}