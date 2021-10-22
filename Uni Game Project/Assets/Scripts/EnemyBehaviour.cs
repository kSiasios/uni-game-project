using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    enum EnemyType
    {
        Walker,
        Flyer
    }

    [Header("Core Elements")]
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] CapsuleCollider2D enemyColider;
    [SerializeField] CircleCollider2D actingAreaColider;

    [SerializeField] GameObject patrolPointPrefab;

    [Header("Enemy Attributes")]
    [Range(1, 100)] [SerializeField] int health = 50;
    [SerializeField] EnemyType enemyType = EnemyType.Walker;
    [Range(0f, 10f)] [SerializeField] float speed = 10f;
    [Range(5f, 10f)] [SerializeField] float patrolRadius = 1f;

    [HideInInspector] public bool chasingPlayer = false;
    [HideInInspector] public bool patroling = true;
    [HideInInspector] public bool facingRight = true;
     public bool movingTowardsPoint = false;

    public Queue<GameObject> patrolPoints = new Queue<GameObject>();

    private void Awake()
    {
        if (patrolRadius < 5)
        {
            patrolRadius = Random.Range(5f, 10f);
        }

        // If the rigidbody is not initialized, try to find it in the current GameObject
        if (rigidbody == null)
        {
            rigidbody = transform.GetComponent<Rigidbody2D>();
        }

        if (enemyColider == null)
        {
            enemyColider = transform.GetComponent<CapsuleCollider2D>();
        }

        if (actingAreaColider == null)
        {
            actingAreaColider = transform.GetComponent<CircleCollider2D>();
        }

        SetPatrolPoints();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (patroling)
        {
            if (rigidbody.velocity == Vector2.zero)
            {
                movingTowardsPoint = false;
            } 
            if (!movingTowardsPoint)
            {
                GameObject point = patrolPoints.Dequeue();
                movingTowardsPoint = true;
                MoveTowardsPoint(point.transform.position);
                patrolPoints.Enqueue(point);
            }
        }
    }

    void MoveTowardsPoint(Vector2 point)
    {
        // If entity is moving to the left, flip them to face to the left
        if (point.x < transform.position.x && facingRight)
        {
            facingRight = false;
            transform.Rotate(0, -180, 0);
        }
        // If entity is moving to the right, flip them to face to the right
        else if (point.x > transform.position.x && !facingRight)
        {
            facingRight = true;
            transform.Rotate(0, 180, 0);
        } else if ((point.x >= transform.position.x && facingRight) || (point.x <= transform.position.x && !facingRight))
        {
            movingTowardsPoint = false;
        }

        if (enemyType == EnemyType.Walker)
        {
            rigidbody.velocity = new Vector2(point.x * speed, rigidbody.velocity.y);
        } else if (enemyType == EnemyType.Flyer)
        {
            rigidbody.velocity = new Vector2(point.x * speed, point.y * speed);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            chasingPlayer = true;
            patroling = false;
            ChasePlayer(collision.transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Ground")
        {
            // We are in an obstacle
            movingTowardsPoint = false;
        }
    }

    // Function that creates the patrol points of the Entity
    void SetPatrolPoints()
    {
        if (enemyType == EnemyType.Walker)
        {
            Debug.Log("Setting Patrol Points for Walker");
            // Create 2 GameObjects to store the patrol points
            GameObject patrolPoint1;
            GameObject patrolPoint2;
            // Instantiate new patrol points and assign them to the GameObjects
            patrolPoint1 = Instantiate(patrolPointPrefab, transform.position, transform.rotation);
            patrolPoint1.name = "PatrolPoint1";
            patrolPoints.Enqueue(patrolPoint1);

            patrolPoint2 = Instantiate(patrolPointPrefab, transform.position, transform.rotation);
            patrolPoint2.name = "PatrolPoint2";
            patrolPoints.Enqueue(patrolPoint2);

            // Set the point to 
            patrolPoint1.GetComponent<PatrolPoint>().SetPatrolPoint(-patrolRadius);
            patrolPoint2.GetComponent<PatrolPoint>().SetPatrolPoint(patrolRadius);

        }
    }

    // Function that handles chasing the player
    void ChasePlayer(Vector2 playerPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
        Debug.Log("Chasing Player");
    }

    // Function handling the health of the Entity
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    // Function that destroys the GameObject
    void Death()
    {
        Debug.Log("Enemy Dead");
        Destroy(this.gameObject);
    }
}
