using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperCharacter : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public GameObject explosionEffectPrefab;
    public GameObject helperCharacterPrefab;
    public float spawnRadius = 5f;
    public int spawnCount = 1;

    public int maxLife = 10;
    private int currentLife;

    private Transform playerTransform;
    private Transform targetEnemy;
    private bool isAttacking = false;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentLife = maxLife;
        StartCoroutine(AutoAttack());
        SpawnHelperCharacters();
    }

    void Update()
    {
        if (currentLife <= 0)
        {
            // HelperCharacter life has run out, destroy it
            DestroyHelperCharacter();
        }
        else if (!isAttacking)
        {
            MoveTowardsNearestEnemy();
        }
    }

    private void MoveTowardsNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                targetEnemy = nearestEnemy.transform;
                Vector2 direction = targetEnemy.position - transform.position;
                transform.Translate(direction.normalized * movementSpeed * Time.deltaTime);
            }
        }
    }

    private void TriggerExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator AutoAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldown);
            Attack();
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DestroyEnemy();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        currentLife--;
        // Perform any necessary actions when the HelperCharacter takes damage
        // For example, you can update a health bar UI element or play a sound effect

        if (currentLife <= 0)
        {
            // HelperCharacter life has run out, destroy it
            DestroyHelperCharacter();
        }
    }

    private void DestroyHelperCharacter()
    {
        // Trigger explosion effect before destroying the HelperCharacter
        TriggerExplosionEffect();

        // Destroy the HelperCharacter game object
        Destroy(gameObject);
    }

    private void SpawnHelperCharacters()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Calculate the spawn position
            float angle = i * (360f / spawnCount);
            Vector2 spawnPosition = transform.position + Quaternion.Euler(0f, 0f, angle) * Vector2.up * spawnRadius;

            // Instantiate the HelperCharacter at the spawn position
            GameObject helperCharacter = Instantiate(helperCharacterPrefab, spawnPosition, Quaternion.identity);
            HelperCharacter helperCharacterComponent = helperCharacter.GetComponent<HelperCharacter>();

            // Set the target enemy for the spawned HelperCharacter
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length > 0)
            {
                GameObject nearestEnemy = null;
                float shortestDistance = Mathf.Infinity;

                foreach (GameObject enemy in enemies)
                {
                    float distance = Vector2.Distance(helperCharacter.transform.position, enemy.transform.position);

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }

                if (nearestEnemy != null)
                {
                    helperCharacterComponent.SetTargetEnemy(nearestEnemy.transform);
                }
            }
        }
    }

    private void MoveTowardsEnemy()
    {
        Vector2 direction = targetEnemy.position - transform.position;
        transform.Translate(direction.normalized * movementSpeed * Time.deltaTime);
    }

    public void SetTargetEnemy(Transform enemyTransform)
    {
        targetEnemy = enemyTransform;
    }
}
