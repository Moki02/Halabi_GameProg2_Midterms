using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color[] colors = { Color.red, Color.green, Color.blue };
    private int currentColorIndex = 0;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 2f;
    public float rotationSpeed = 2f;
    public float detectionRadius = 5f;
    public float checkInterval = 0.5f; // check every 0.5s
    public float shootInterval = 1f; // shoot every 1s if enemy exists

    private GameObject currentTarget;
    private Coroutine shootingCoroutine;
    private int enemiesDestroyed = 0;
    public TextMeshProUGUI enemiesDestroyedText;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colors[currentColorIndex];
        UpdateEnemiesDestroyedText();
        StartCoroutine(CheckEnemiesRoutine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ChangeColor();

        if (currentTarget != null)
            SmoothFaceTarget();
    }

    private IEnumerator CheckEnemiesRoutine()
    {
        while (true)
        {
            DetectEnemies();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void DetectEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float nearestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestEnemy = hit.gameObject;
                }
            }
        }

        // If new target is detected, update
        if (nearestEnemy != currentTarget)
        {
            currentTarget = nearestEnemy;

            // Start shooting if an enemy is detected
            if (currentTarget != null)
            {
                if (shootingCoroutine == null)
                    shootingCoroutine = StartCoroutine(ShootingRoutine());
            }
            else
            {
                // Stop shooting if no enemy detected
                if (shootingCoroutine != null)
                {
                    StopCoroutine(shootingCoroutine);
                    shootingCoroutine = null;
                }
            }
        }

        // If no enemies remain, stop shooting
        if (nearestEnemy == null && shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (currentTarget != null)
        {
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
        }

        shootingCoroutine = null; // reset when done
    }

    private void SmoothFaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, targetAngle));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void ShootBullet()
    {
        if (currentTarget == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<SpriteRenderer>().color = spriteRenderer.color;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * bulletSpeed;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetTower(this);

        Destroy(bullet, 5f);
    }

    private void ChangeColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        spriteRenderer.color = colors[currentColorIndex];
    }

    public void IncrementEnemiesDestroyed()
    {
        enemiesDestroyed++;
        UpdateEnemiesDestroyedText();
    }

    private void UpdateEnemiesDestroyedText()
    {
        if (enemiesDestroyedText != null)
            enemiesDestroyedText.text = " " + enemiesDestroyed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
