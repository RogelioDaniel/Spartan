using System.Collections;
using UnityEngine;

public class AttackRangeItem : MonoBehaviour
{
    public float attackRangeIncreaseAmount = 1.0f;
    public float duration = 10.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                // Increase attack range
                player.IncreaseAttackRange(attackRangeIncreaseAmount, duration);

                // Destroy the item
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("HelperCharacter"))
        {
            HelperCharacter helperCharacter = collision.GetComponent<HelperCharacter>();
            if (helperCharacter != null)
            {
                // Increase attack range
                helperCharacter.IncreaseAttackRange(attackRangeIncreaseAmount, duration);

                // Destroy the item
                Destroy(gameObject);
            }
        }
    }
}
