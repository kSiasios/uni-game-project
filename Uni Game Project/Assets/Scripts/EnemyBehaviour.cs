using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyType
    {
        Walker,
        Flyer
    }

    [Header("Core Elements")]
    [Tooltip("The rigidbody of the enemy")]
    [SerializeField] new Rigidbody2D rigidbody;
    [Tooltip("The collider of the enemy responsible for detecting collisions")]
    [SerializeField] CapsuleCollider2D enemyCollider;
    [Tooltip("The collider of the enemy responsible for detecting the player when in range")]
    [SerializeField] CircleCollider2D actingAreaCollider;

    [SerializeField] GameObject patrolPointPrefab;

    [Header("Enemy Attributes")]
    [Tooltip("How much health does the enemy have")]
    [Range(1, 100)] [SerializeField] int health = 50;
    [Tooltip("What is the type of the enemy")]
    public EnemyType enemyType = EnemyType.Walker;
    [Tooltip("How quickly will the enemy move")]
    [Range(0f, 10f)] [SerializeField] float speed = 10f;
    [Tooltip("How far will the patrol points be from the center of the enemy")]
    [Range(5f, 10f)] [SerializeField] float patrolRadius = 1f;
    [Tooltip("How much time (in seconds) should the enemy wait before moving on to the next patrol point")]
    [Range(0f, 5f)] [SerializeField] float waitOnStop = 0.5f;
    [Tooltip("How many patrolpoints will the flyer have. Ignore if enemy is walker")]
    [Range(1, 10)] [SerializeField] int patrolPointsNumber = 10;
    private float waitTimer = 0f;
    private bool timePassed = false;
    private float gravityScale = 0f;

    [HideInInspector] public bool chasingPlayer = false;
    [HideInInspector] public bool patroling = true;
    [HideInInspector] public bool facingRight = true;
    public bool movingTowardsPoint = false;

    GameObject targetPos;

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

        if (enemyCollider == null)
        {
            enemyCollider = transform.GetComponent<CapsuleCollider2D>();
        }

        if (actingAreaCollider == null)
        {
            actingAreaCollider = transform.GetComponent<CircleCollider2D>();
        }

        gravityScale = rigidbody.gravityScale;

        SetPatrolPoints();
    }

    private void Update()
    {
        if (Time.unscaledTime - waitTimer >= waitOnStop)
        {
            timePassed = true;
            rigidbody.gravityScale = gravityScale;
        }
        else
        {
            timePassed = false;
        }
    }

    private void FixedUpdate()
    {
        if (patroling)
        {
            if (timePassed)
            {
                if (rigidbody.velocity == Vector2.zero)
                {
                    movingTowardsPoint = false;
                }
                if (!movingTowardsPoint)
                {
                    GameObject point = patrolPoints.Dequeue();
                    movingTowardsPoint = true;
                    //MoveTowardsPoint(targetPos.transform.position);
                    targetPos = point;
                    patrolPoints.Enqueue(point);
                }
                MoveTowardsPoint(targetPos.transform.position);
            }
        }
    }

    void MoveTowardsPoint(Vector2 point)
    {
        // Normalize() will return a vector with values from 0 to 1,
        // suitable for multiplication
        Vector2 normalizedVector = point - new Vector2(transform.position.x, transform.position.y);
        normalizedVector.Normalize();

        // Flip the entity
        if (normalizedVector.x < 0)
        {
            // Change the players rotation to face the correct way
            if (facingRight)
            {
                transform.Rotate(0, 180, 0);
                facingRight = false;
            }
        }
        else if (normalizedVector.x > 0)
        {
            if (!facingRight)
            {
                transform.Rotate(0, -180, 0);
                facingRight = true;
            }
        }

        if (enemyType == EnemyType.Walker)
        {
            // we dont need to move on the Y axis
            Vector2 walkerDirection = new Vector2(normalizedVector.x, 0);

            // If the enemy is not too close to the end, continue moving
            //Debug.Log(Mathf.Abs(transform.position.x - point.x));
            if (Mathf.Abs(transform.position.x - point.x) > 1f)
            {
                rigidbody.velocity = new Vector2(walkerDirection.x * speed, rigidbody.velocity.y);
            }
            // If enemy is too close to the end, set their velocity to zero
            else
            {
                rigidbody.velocity = Vector2.zero;
                // Setting the gravityScale to 0, will prevent the enemy
                // from falling off a ramp and setting up a loop of trying to get to the point
                rigidbody.gravityScale = 0;
                waitTimer = Time.unscaledTime;
            }
        }
        if (enemyType == EnemyType.Flyer)
        {
            // we dont need to move on the Y axis
            Vector2 flyerDirection = new Vector2(normalizedVector.x, normalizedVector.y);

            // If the enemy is not too close to the end, continue moving
            if (Mathf.Abs(transform.position.x - point.x) > 0.5f)
            {
                rigidbody.velocity = new Vector2(flyerDirection.x * speed, flyerDirection.y * speed);
            }
            // If enemy is too close to the end, set their velocity to zero
            else
            {
                rigidbody.velocity = Vector2.zero;
                waitTimer = Time.unscaledTime;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // Player is in range, chase them
            chasingPlayer = true;
            patroling = false;
            ChasePlayer(collision.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            // Player is not in range, get back to patrolling
            chasingPlayer = false;
            patroling = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Ground")
        {
            // We are in an obstacle
            movingTowardsPoint = false;
            waitTimer = Time.unscaledTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (rigidbody.velocity.x < speed * .5f && rigidbody.velocity.y < speed * .5f && enemyType == EnemyType.Flyer)
        {
            // We are going to get stuck on an obstacle
            movingTowardsPoint = false;
            waitTimer = Time.unscaledTime;
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
            Debug.Log(transform.position);

            patrolPoint1 = Instantiate(patrolPointPrefab, transform.position, transform.rotation);
            patrolPoint1.name = "PatrolPoint1";
            patrolPoints.Enqueue(patrolPoint1);

            patrolPoint2 = Instantiate(patrolPointPrefab, transform.position, transform.rotation);
            patrolPoint2.name = "PatrolPoint2";
            patrolPoints.Enqueue(patrolPoint2);

            // Set the point to a location
            patrolPoint1.GetComponent<PatrolPoint>().SetPatrolPoint(-patrolRadius);
            patrolPoint2.GetComponent<PatrolPoint>().SetPatrolPoint(patrolRadius);

        }
        else if (enemyType == EnemyType.Flyer)
        {
            Debug.Log("Setting Patrol Points for Flyer");
            // For each patrol point of the flyer,
            //      create a gameobject,
            //      put it in the queue of patrolpoints
            //      set its position to a random point in space
            for (int i = 0; i < patrolPointsNumber; i++)
            {
                GameObject patrolPoint = Instantiate(patrolPointPrefab, transform.position, transform.rotation);
                patrolPoints.Enqueue(patrolPoint);
                Vector2 point = new Vector2(Random.Range(1, patrolRadius) * Mathf.Sign(Random.Range(-1, 1)), Random.Range(1, patrolRadius) * Mathf.Sign(Random.Range(-1, 1)));
                patrolPoint.GetComponent<PatrolPoint>().SetPatrolPointFlyer(point);
            }
        }
    }

    // Function that handles chasing the player
    void ChasePlayer(Vector2 playerPos)
    {
        //transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
        Debug.Log("Chasing Player");

        if (enemyType == EnemyType.Flyer)
        {
            // If the enemy is a Flyer, move above the player and shoot projectiles
            Vector2 positionToGo = new Vector2(playerPos.x, playerPos.y + patrolRadius / 2);
            //transform.position = Vector2.MoveTowards(transform.position, positionToGo, speed * Time.deltaTime);
            MoveTowardsPoint(positionToGo);
            //ShootPlayer();

        }
        else if (enemyType == EnemyType.Walker)
        {
            //transform.position = Vector2.MoveTowards(transform.position, playerPos, speed * Time.deltaTime);
            MoveTowardsPoint(playerPos);
        }
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

    public void ShootPlayer()
    {

    }

    // Function that destroys the GameObject
    void Death()
    {
        Debug.Log("Enemy Dead");
        Destroy(this.gameObject);
    }
}
