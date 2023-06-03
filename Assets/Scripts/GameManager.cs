using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Static reference to the GameManager instance

    public GameObject enemy;
    public GameObject helperCharacterPrefab; // Prefab of the HelperCharacter
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;
    public Text scoreText;
    public string restartSceneName = "Game";
    private int score = 0;
    private bool gameStarted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the GameManager instance
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
        }
    }

    void Start()
    {
        
        SpawnHelperCharacters();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            StartSpawning();
            gameStarted = true;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.x = Random.Range(-maxX, maxX);

        Instantiate(enemy, spawnPos, Quaternion.identity);
    }

    private void SpawnHelperCharacters()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int enemyCount = enemies.Length;
        int helperCharacterCount = 29;

        if (enemyCount == 0)
        {
            return; // No enemies to follow, so return
        }

        for (int i = 0; i < helperCharacterCount; i++)
        {
            Vector3 spawnPos = spawnPoint.position;
            spawnPos.x = Random.Range(-maxX, maxX);

            GameObject helperCharacter = Instantiate(helperCharacterPrefab, spawnPos, Quaternion.identity);
            HelperCharacter helperCharacterComponent = helperCharacter.GetComponent<HelperCharacter>();

            int enemyIndex = i % enemyCount; // Get the index of the target enemy

            helperCharacterComponent.SetTargetEnemy(enemies[enemyIndex].transform);
        }
    }

    private void StartSpawning()
    {
        InvokeRepeating("SpawnEnemy", 1.5f, spawnRate);
        Invoke("SpawnHelperCharacters", 2f); // Invoke the method to spawn HelperCharacters after a delay
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(restartSceneName);
    }

    public void EliminateEnemies(int count)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int eliminatedCount = 0;

        foreach (GameObject enemy in enemies)
        {
            if (eliminatedCount >= count)
                break;

            Destroy(enemy);
            eliminatedCount++;
        }
    }
}