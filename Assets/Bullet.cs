using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PlayerController tower;

    public void SetTower(PlayerController towerRef)
    {
        tower = towerRef;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Simulate enemy death
            Destroy(collision.gameObject);

            // Tell tower enemy was destroyed
            if (tower != null)
                tower.IncrementEnemiesDestroyed();

            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
