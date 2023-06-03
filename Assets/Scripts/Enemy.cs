using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform target;
    public SpriteRenderer enemySprite;
    public string restartSceneName = "Game";
    public float fadeDuration = 1.5f;
    public GameObject dropItemPrefab; //
    public GameObject destroyEffectPrefab; //

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
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
        GameObject destroyEffect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);

        // Check if the enemy should drop an item
        if (dropItemPrefab != null)
        {
            // Instantiate the item at the enemy's position
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
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
}
