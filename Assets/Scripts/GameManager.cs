using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Static reference to the GameManager instance

    public GameObject enemy;
    public GameObject helperCharacterPrefab; // Prefab of the HelperCharacter
    public GameObject itemPrefab; // Prefab of the item
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;
    public Text scoreText;
    public string restartSceneName = "Principal";
    private int score = 0;
    private bool gameStarted = false;
    private GameObject player;
    private int waveNumber = 1;
    public Text waveText;
    private List<HelperCharacter> helperCharacters = new List<HelperCharacter>();
    public int maxEnemyCount = 10; // Maximum number of enemies allowed
    public float spawnDelay = 1.0f; // Delay between enemy spawns
    private int currentEnemyCount = 0; // Current number of enemies spawned
    private int enemiesDestroyed = 0;
    public int enemiesPerItem ; // Number of enemies to destroy before spawning an item
    public Text startText;
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
        player = GameObject.FindGameObjectWithTag("Player");
        SpawnHelperCharacters();
        waveText.text = "Wave " + waveNumber;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        startText.gameObject.SetActive(false); // Hide the start text

        StartCoroutine(SpawnWave());
        Invoke("SpawnHelperCharacters", 0.1f); // Invoke the method to spawn HelperCharacters after a delay
    }

    private void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemyCount)
        {
            return; // Stop spawning if the maximum enemy count is reached
        }

        Vector3 spawnPos = GetRandomSpawnPosition();
        Instantiate(enemy, spawnPos, Quaternion.identity);

        currentEnemyCount++;
        DecreaseEnemyCount();
    }

    public void DecreaseEnemyCount()
    {
        currentEnemyCount--;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float spawnX = Random.Range(-maxX, maxX);
        float spawnY = spawnPoint.position.y + spawnPoint.localScale.y / 2f;

        return new Vector3(spawnX, spawnY, 0f);
    }

    private void SpawnItem()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        Instantiate(itemPrefab, spawnPos, Quaternion.identity);
    }

    private void SpawnHelperCharacters()
    {
        int helperCharacterCount = 9;

        for (int i = 0; i < helperCharacterCount; i++)
        {
            float angle = i * (360f / helperCharacterCount);
            Vector3 spawnPosition = player.transform.position + Quaternion.Euler(0f, 0f, angle) * Vector3.up * maxX;

            GameObject helperCharacterObject = Instantiate(helperCharacterPrefab, spawnPosition, Quaternion.identity);
            HelperCharacter helperCharacterComponent = helperCharacterObject.GetComponent<HelperCharacter>();
            helperCharacterComponent.SetTargetPlayer(player.transform);
            helperCharacters.Add(helperCharacterComponent);
        }
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnWave());
        Invoke("SpawnHelperCharacters", 0.1f); // Invoke the method to spawn HelperCharacters after a delay
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;

        enemiesDestroyed++;

        if (enemiesDestroyed % enemiesPerItem == 0)
        {
            SpawnItem();
        }
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

    public void UpdateHelperCharactersTarget()
    {
        foreach (HelperCharacter helperCharacter in helperCharacters)
        {
            helperCharacter.SetTargetPlayer(player.transform);
        }
    }

    private IEnumerator SpawnWave()
    {
        int initialEnemiesToSpawn = 10; // Number of enemies to spawn in the first wave
        int enemiesToSpawn = initialEnemiesToSpawn;
        float spawnRateMultiplier = 7.8f; // Rate at which the spawn rate increases with each wave

        while (true)
        {
            // Calculate the spawn rate based on the wave number
            float currentSpawnRate = spawnRate / (waveNumber * spawnRateMultiplier);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Get a random position within the map bounds
                Vector3 spawnPos = new Vector3(Random.Range(-maxX, maxX), spawnPoint.position.y, spawnPoint.position.z);

                GameObject newEnemy = Instantiate(enemy, spawnPos, Quaternion.identity);
                newEnemy.SetActive(true);

                currentEnemyCount++; // Increase the enemy count

                yield return new WaitForSeconds(spawnDelay);
            }

            waveNumber++;
            waveText.text = "Wave " + waveNumber;

            // Increase the number of enemies to spawn in the next wave
            enemiesToSpawn++;

            yield return new WaitForSeconds(currentSpawnRate);
        }
    }
}
