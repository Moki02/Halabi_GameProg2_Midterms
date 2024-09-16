using UnityEngine;

public class TriangleSpawner : MonoBehaviour
{
    public GameObject trianglePrefab; // Reference to the triangle prefab
    public Transform player; // Reference to the player triangle
    public float initialSpawnInterval = 2f; // Initial time between spawns
    public float spawnIntervalDecreaseRate = 0.1f; // Rate at which the spawn interval decreases over time
    public float minimumSpawnInterval = 0.5f; // Minimum allowed spawn interval
    public float spawnRadius = 10f; // Distance from player where triangles will spawn
    public float triangleSpeed = 1f; // Speed of the triangles moving toward the player

    private Color[] colors = { Color.red, Color.green, Color.blue};
    private float spawnTimer = 0f; // Timer to track time since last spawn
    private float currentSpawnInterval; // Current interval between spawns

    void Start()
    {
        // Set the current spawn interval to the initial value
        currentSpawnInterval = initialSpawnInterval;
    }

    void Update()
    {
        // Increase the timer
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn a new triangle
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnTriangle();
            spawnTimer = 0f; // Reset the timer

            // Gradually decrease the spawn interval to speed up enemy spawning
            if (currentSpawnInterval > minimumSpawnInterval)
            {
                currentSpawnInterval -= spawnIntervalDecreaseRate * Time.deltaTime;
            }
        }
    }

    void SpawnTriangle()
    {
        // Determine a random position around the player to spawn the triangle
        Vector2 spawnPosition = (Vector2)player.position + Random.insideUnitCircle.normalized * spawnRadius;

        // Instantiate a new triangle
        GameObject triangle = Instantiate(trianglePrefab, spawnPosition, Quaternion.identity);

        // Assign a random color to the triangle
        SpriteRenderer triangleRenderer = triangle.GetComponent<SpriteRenderer>();
        if (triangleRenderer != null)
        {
            triangleRenderer.color = colors[Random.Range(0, colors.Length)];
        }

        // Make the triangle move toward the player
        triangle.AddComponent<MoveTowardsPlayer>().Initialize(player, triangleSpeed);
    }
}
