using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Check if the bullet color matches the enemy color
            if (collision.GetComponent<SpriteRenderer>().color == GetComponent<SpriteRenderer>().color)
            {
                Destroy(collision.gameObject); // Destroy the enemy if the colors match
                FindObjectOfType<PlayerController>().IncrementEnemiesDestroyed(); // Increment enemy count
            }

            Destroy(gameObject); // Destroy the bullet on impact
        }
    }
}
