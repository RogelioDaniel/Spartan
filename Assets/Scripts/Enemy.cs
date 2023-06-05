using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enemyCount++;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * speed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
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
