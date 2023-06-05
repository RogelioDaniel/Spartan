using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab of the enemy to be pooled
    public int poolSize = 100; // Number of enemies to be pooled

    private List<GameObject> pooledEnemies = new List<GameObject>();

    private void Start()
    {
        // Instantiate and pool the enemies
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            pooledEnemies.Add(enemy);
        }
    }

    public GameObject GetPooledEnemy()
    {
        // Search for an inactive enemy in the pool
        foreach (GameObject enemy in pooledEnemies)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        // If all enemies are active, create a new one and add it to the pool
        GameObject newEnemy = Instantiate(enemyPrefab);
        pooledEnemies.Add(newEnemy);
        return newEnemy;
    }
}
