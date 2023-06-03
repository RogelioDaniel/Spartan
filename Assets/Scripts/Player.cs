using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int maxLife = 300;
    private int currentLife;
    public float speed = 5f;
    public float attackRange = 1.5f;
    private int scoreToAdd = 100;
    public LayerMask enemyLayer;
    public JoystickController joystickController;
 
    public Animator animator;
    private Vector2 movementDirection;
    public AudioClip enemyDefeatSound; // Sound effect to play when the enemy is defeated

    private AudioSource audioSource; // Reference to the AudioSource component

    private Rigidbody2D rb;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentLife = 100;
        StartCoroutine(AutoAttack());

    }

    private void Update()
    {

        movementDirection = joystickController.joystickVec;
        MovePlayer();
        if (currentLife <= 0)
        {
            // Handle player death here
            // For example, you can trigger a game over state or restart the game
            GameManager.Instance.RestartGame();
            return;
        }

        if (joystickController.joystickVec.y != 0)
        {
            rb.velocity = new Vector2(joystickController.joystickVec.x * speed, joystickController.joystickVec.y * speed);
            RotatePlayerTowardsNearestEnemy();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // Update the position of the life text above the sprite
        

        UpdateAnimation();
    }

    private IEnumerator AutoAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Attack();
        }
    }

    private void RotatePlayerTowardsNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            GameObject nearestEnemy = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                Vector2 direction = nearestEnemy.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DestroyEnemy();
                GameManager.Instance.IncreaseScore(scoreToAdd);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
            DefeatEnemy();
        }
    }

    private void TakeDamage()
    {
        currentLife--;
        // Update UI or perform any necessary actions when the player takes damage
        // For example, you can update a health bar UI element or play a sound effect

        Debug.Log("Player life: " + currentLife);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void MovePlayer()
    {
        rb.velocity = movementDirection * speed;
    }

    private void UpdateAnimation()
    {
        // Calculate the absolute horizontal and vertical movement values
        float horizontalMovement = Mathf.Abs(movementDirection.x);
        float verticalMovement = Mathf.Abs(movementDirection.y);

        // Check if the player is moving horizontally or vertically
        if (horizontalMovement > verticalMovement)
        {
            // Set the animator parameter for horizontal movement
            animator.SetFloat("MoveX", movementDirection.x);
            animator.SetFloat("MoveY", 0f);
        }
        else if (verticalMovement > horizontalMovement)
        {
            // Set the animator parameter for vertical movement
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", movementDirection.y);
        }

        // Set the magnitude of the movement vector as the animator parameter for movement speed
        animator.SetFloat("Speed", movementDirection.magnitude);
    }
    public void DefeatEnemy()
    {
        Attack();
        // Play the sound effect when the enemy is defeated
        audioSource.PlayOneShot(enemyDefeatSound);
    }
}