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
    public float maxX;
    public Transform spawnPoint;
    public float spawnRate;
    public Text scoreText;
    public string restartSceneName = "Game";
    private int score = 0;
    private bool gameStarted = false;
    private GameObject player;
    private int waveNumber = 1;
    public Text waveText;
    private List<HelperCharacter> helperCharacters = new List<HelperCharacter>();

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
            StartSpawning();
            gameStarted = true;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = spawnPoint.position;
        spawnPos.y = spawnPoint.position.y + spawnPoint.localScale.y / 2f;

        Instantiate(enemy, spawnPos, Quaternion.identity);
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
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            for (int i = 0; i < waveNumber; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnRate);
            }

            waveNumber++;
            waveText.text = "Wave " + waveNumber;
        }
    }
}