using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    // Genetic Algorithm Parameters
    public int chromosomeLength = 10; // Length of the attack behavior chromosome
    public float mutationRate = 0.1f; // Mutation rate for the genetic algorithm

    // Other variables
    public float speed = 2f;
    public Transform target;
    public SpriteRenderer enemySprite;
    public string restartSceneName = "Principal";
    public float fadeDuration = 1.5f;
    public GameObject itemPrefab;
    public GameObject explosionEffectPrefab;
    private int enemyCount = 0;
    public int enemiesPerItemDrop = 10; // Number of enemies to destroy before dropping an item

    private Rigidbody2D rb;
    private string attackBehavior; // The attack behavior chromosome

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enemyCount++;

        // Initialize the attack behavior chromosome
        InitializeAttackBehavior();
    }

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            // Execute the attack behavior
            ExecuteAttackBehavior();
        }
    }

    private void InitializeAttackBehavior()
    {
        // Generate a random attack behavior chromosome
        attackBehavior = GenerateRandomChromosome();
    }

    private string GenerateRandomChromosome()
    {
        string chromosome = "";

        for (int i = 0; i < chromosomeLength; i++)
        {
            // Randomly assign attack actions to the chromosome
            int randomAction = Random.Range(0, 2); // Example: 0 - Do nothing, 1 - Attack

            chromosome += randomAction.ToString();
        }

        return chromosome;
    }

    private void ExecuteAttackBehavior()
    {
        // Execute the attack behavior based on the current frame index
        int frameIndex = Mathf.FloorToInt(Time.time * 10); // Change attack behavior every 0.1 seconds

        if (frameIndex >= attackBehavior.Length)
        {
            // Regenerate the attack behavior chromosome if it exceeds the length
            attackBehavior = GenerateRandomChromosome();
        }
        else
        {
            // Get the action at the current frame index
            int action = int.Parse(attackBehavior[frameIndex].ToString());

            if (action == 1)
            {
                // Perform the attack action
                Attack();
            }
        }
    }

    private void Attack()
    {
        // Implement the attack logic here
        // This is where you define how the enemy attacks the player
        // For example, you can instantiate projectiles, perform raycasts, etc.
    }

    public void DestroyEnemy()
    {
        // Instantiate the destroy effect at the enemy's position
        GameObject destroyEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // Decrease the enemy count
        enemyCount++;

        // Check if the enemy count is divisible by the enemiesPerItemDrop
        if (enemyCount % enemiesPerItemDrop == 0)
        {
            print("entrada if" + enemyCount);
            SpawnItem();
        }

        // Destroy the enemy game object
        Destroy(gameObject);

        // Destroy the destroy effect prefab after a delay
        Destroy(destroyEffect, 0.5f);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(restartSceneName);
    }

    private void SpawnItem()
    {
        Vector3 spawnPosition = transform.position;
        Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }
}
