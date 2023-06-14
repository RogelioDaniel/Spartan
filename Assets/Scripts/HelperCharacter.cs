using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperCharacter : MonoBehaviour
{
    public float movementSpeed ;
    public float attackRange ;
    public float attackCooldown;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public GameObject explosionEffectPrefab;

    public float spawnRadius ;
    public int spawnCount ;
    private Transform targetPlayer;

    public int maxLife ;
    private int currentLife;

    private Transform playerTransform;
    private Transform targetEnemy;
    private bool isAttacking = false;
    private bool isAttackRangeIncreased = false;
    private float originalAttackRange;
    private Coroutine attackRangeCoroutine;

    public float barrierDistance ; // Distance between the player and the Helper Character while forming the barrier

    // Genetic Algorithm Parameters
    private int chromosomeLength = 10; // Length of the movement behavior chromosome
    private string movementBehavior; // Movement behavior chromosome

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentLife = maxLife;
        StartCoroutine(AutoAttack());

        InitializeMovementBehavior();
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
                // Execute the movement behavior
                ExecuteMovementBehavior();

                // Detect nearby enemies
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                if (enemies.Length > 0)
                {
                    GameObject nearestEnemy = null;
                    float shortestDistance = Mathf.Infinity;

                    foreach (GameObject enemy in enemies)
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);

                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }

                    if (nearestEnemy != null)
                    {
                        // Move towards the nearest enemy
                        Vector3 enemyDirection = nearestEnemy.transform.position - transform.position;
                        transform.position = Vector3.MoveTowards(transform.position, nearestEnemy.transform.position, movementSpeed * Time.deltaTime);
                        transform.rotation = Quaternion.LookRotation(Vector3.forward, enemyDirection);
                    }
                }
            }
        }
    }

    private void InitializeMovementBehavior()
    {
        movementBehavior = GetMovementBehaviorChromosome();
    }

    private void ExecuteMovementBehavior()
    {
        int frameIndex = Mathf.FloorToInt(Time.time * 10); // Change movement behavior every 0.1 seconds

        if (frameIndex >= movementBehavior.Length)
        {
            // Regenerate the movement behavior chromosome if it exceeds the length
            movementBehavior = GenerateRandomChromosome();
        }
        else
        {
            // Get the action at the current frame index
            int action = int.Parse(movementBehavior[frameIndex].ToString());

            if (action == 1)
            {
                // Perform the movement action
                MoveTowardsEnemy();
            }
        }
    }

    private string GetMovementBehaviorChromosome()
    {
        // Retrieve the movement behavior chromosome for this HelperCharacter
        // You can implement your own logic to get the chromosome, such as from a database or file
        // For simplicity, this example generates a random chromosome
        return GenerateRandomChromosome();
    }

    private string GenerateRandomChromosome()
    {
        // Generate a random movement behavior chromosome
        string chromosome = "";

        for (int i = 0; i < chromosomeLength; i++)
        {
            chromosome += Random.Range(0, 2).ToString();
        }

        return chromosome;
    }

    public void IncreaseAttackRange(float amount, float duration)
    {
        if (isAttackRangeIncreased)
        {
            StopCoroutine(attackRangeCoroutine);
            isAttackRangeIncreased = false;
            ResetAttackRange();
        }

        attackRangeCoroutine = StartCoroutine(IncreaseAttackRangeCoroutine(amount, duration));
    }

    private IEnumerator IncreaseAttackRangeCoroutine(float amount, float duration)
    {
        isAttackRangeIncreased = true;
        originalAttackRange = attackRange;

        attackRange += amount;

        yield return new WaitForSeconds(duration);

        isAttackRangeIncreased = false;
        ResetAttackRange();
    }

    private void ResetAttackRange()
    {
        attackRange = originalAttackRange;
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
        if (targetEnemy != null)
        {
            Vector2 direction = targetEnemy.position - transform.position;
            Vector2 forward = transform.up;

            float angle = Vector2.Angle(forward, direction);

            if (angle < 180f) // Only attack if enemy is in front (within 90 degrees)
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 collisionDirection = collision.transform.position - transform.position;
            Vector2 forward = transform.up;

            float angle = Vector2.Angle(forward, collisionDirection);

            if (angle > 90f) // Only take damage if hit from behind (greater than 90 degrees)
            {
                TakeDamage();

            }
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
        if (targetPlayer != null)
        {
            Vector2 direction = targetPlayer.position - transform.position;
            transform.up = direction.normalized;
        }
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
