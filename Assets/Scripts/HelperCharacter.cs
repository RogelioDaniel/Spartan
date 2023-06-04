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

    public float spawnRadius = 5f;
    public int spawnCount = 6;
    private Transform targetPlayer;

    public int maxLife = 10;
    private int currentLife;

    private Transform playerTransform;
    private Transform targetEnemy;
    private bool isAttacking = false;

    public float barrierDistance = 2f; // Distance between the player and the Helper Character while forming the barrier

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentLife = maxLife;
        StartCoroutine(AutoAttack());
    }

    void Update()
    {
        if (currentLife <= 0)
        {
            // HelperCharacter life has run out, destroy it
            DestroyHelperCharacter();
        }

        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer > barrierDistance)
            {
                // Move towards the player
                transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, movementSpeed * Time.deltaTime);
            }
            else
            {
                MoveTowardsNearestEnemy();
                // Stop moving
                // You can also perform other actions here, such as forming the barrier
                // ...
            }
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

    private void MoveTowardsEnemy()
    {
        Vector2 direction = targetEnemy.position - transform.position;
        transform.Translate(direction.normalized * movementSpeed * Time.deltaTime);
    }

    public void SetTargetEnemy(Transform enemyTransform)
    {
        targetEnemy = enemyTransform;
    }

    public void SetTargetPlayer(Transform playerTransform)
    {
        targetPlayer = playerTransform;
    }
}
