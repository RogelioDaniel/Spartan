using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeItem : MonoBehaviour
{
    public float attackRangeIncreaseAmount ;
    public float duration ;
    public GameObject helperCharacterPrefab;
    public int numberOfHelpersToSpawn ;
    public int playerLifeIncreaseAmount; // Amount to increase the player's life
    private List<HelperCharacter> helperCharacters = new List<HelperCharacter>();
    private GameObject player;
    public float maxX;
    private void OnTriggerEnter2D(Collider2D collision)
        
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Increase player's life
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.IncreaseLife(playerLifeIncreaseAmount);
            }

            // Spawn helper characters
            for (int i = 0; i < numberOfHelpersToSpawn; i++)
            {
                SpawnHelperCharacters();
            }

            // Destroy the item
            Destroy(gameObject);
        }
    }

  
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
       
    }

    private void SpawnHelperCharacters()
    {
        Vector3 centerPosition = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < numberOfHelpersToSpawn; i++)
        {
            float angle = i * (360f / numberOfHelpersToSpawn);
            Vector3 spawnPosition = centerPosition + Quaternion.Euler(0f, 0f, angle) * Vector3.up * maxX;

            GameObject helperCharacterObject = Instantiate(helperCharacterPrefab, spawnPosition, Quaternion.identity);
            HelperCharacter helperCharacterComponent = helperCharacterObject.GetComponent<HelperCharacter>();
            helperCharacterComponent.SetTargetPlayer(player.transform);
            helperCharacters.Add(helperCharacterComponent);
        }
    }

    void Update()
    {
         
    }
}
