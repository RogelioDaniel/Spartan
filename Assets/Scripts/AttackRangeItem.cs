using System.Collections;
using UnityEngine;

public class AttackRangeItem : MonoBehaviour
{
    public float attackRangeIncreaseAmount = 1.0f;
    public float duration = 10.0f;
    public GameObject helperCharacterPrefab;
    public int numberOfHelpersToSpawn = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Spawn helper characters
            for (int i = 0; i < numberOfHelpersToSpawn; i++)
            {
                SpawnHelperCharacter();
            }

            // Destroy the item
            Destroy(gameObject);
        }
    }

    private void SpawnHelperCharacter()
    {
        // Instantiate a new helper character at a random position
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 2f; // Adjust the spawn radius as needed
        Instantiate(helperCharacterPrefab, spawnPosition, Quaternion.identity);

    }
}
