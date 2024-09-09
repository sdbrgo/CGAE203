// BulletController.cs -- Put this in the Bullet Prefab itself

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController3 : MonoBehaviour
{
    public float speed = 30f;

    void Update()
    {
        // Move the bullet vertically; change "Vector2.<direction>" to change direction
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Destroy the bullet if it goes off-screen
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            // Destroy(gameObject);
        }
    }
}