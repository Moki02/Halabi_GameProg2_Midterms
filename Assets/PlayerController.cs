using UnityEngine;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color[] colors = { Color.red, Color.green, Color.blue};
    private int currentColorIndex = 0;

    public GameObject bulletPrefab; // Reference to the bullet prefab
    public Transform bulletSpawnPoint; // Reference to the spawn point of the bullet
    public float bulletSpeed = 2f; // Reduced speed of the bullet
    public float shootInterval = 0.5f; // Time between shots

    public float rotationSpeed = 2f; // Speed of rotation towards the enemy

    private float shootTimer = 0f; // Timer to control shooting
    private HashSet<GameObject> enemiesInRange = new HashSet<GameObject>(); // Track enemies in range
    private int enemiesDestroyed = 0; // Counter for enemies destroyed

    public TextMeshProUGUI enemiesDestroyedText; // Reference to TextMeshProUGUI element to display the count

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the initial color
        spriteRenderer.color = colors[currentColorIndex];
        UpdateEnemiesDestroyedText();
    }

    void Update()
    {
        // Change color on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            ChangeColor();
        }

        // Make the player slowly face the nearest enemy
        SmoothFaceNearestEnemy();

        // Check if there are enemies in range and shoot periodically
        if (enemiesInRange.Count > 0)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                ShootBullet();
                shootTimer = 0f;
            }
        }
    }

    private void ChangeColor()
    {
        // Cycle through colors
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        spriteRenderer.color = colors[currentColorIndex];
    }

    public void ShootBullet()
    {
        // Instantiate the bullet and set its color
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<SpriteRenderer>().color = spriteRenderer.color;
        bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;

        // Set bullet to destroy after a certain amount of time to avoid memory leaks
        Destroy(bullet, 5f); // Destroy bullet after 5 seconds if it does not hit anything
    }

    private void SmoothFaceNearestEnemy()
    {
        // Find the nearest enemy from all enemies in the scene
        GameObject nearestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Find all enemies in the scene
        foreach (GameObject enemy in allEnemies)
        {
            Vector3 directionToTarget = enemy.transform.position - currentPosition;
            float distanceSqrToTarget = directionToTarget.sqrMagnitude;
            if (distanceSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqrToTarget;
                nearestEnemy = enemy;
            }
        }

        // Smoothly rotate the player to face the nearest enemy
        if (nearestEnemy != null)
        {
            Vector3 direction = nearestEnemy.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if an enemy enters the detection radius
        if (collision.CompareTag("Enemy"))
        {
            enemiesInRange.Add(collision.gameObject);
            Debug.Log("Enemy entered detection radius: " + collision.gameObject.name); // Log when an enemy enters
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove enemy from the list when it exits the detection radius
        if (collision.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(collision.gameObject);
            Debug.Log("Enemy exited detection radius: " + collision.gameObject.name); // Log when an enemy exits
        }
    }

    public void IncrementEnemiesDestroyed()
    {
        enemiesDestroyed++;
        UpdateEnemiesDestroyedText();
    }

    private void UpdateEnemiesDestroyedText()
    {
        if (enemiesDestroyedText != null)
        {
            enemiesDestroyedText.text = " " + enemiesDestroyed;
        }
    }
}
