using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Core Elements")]
    [Tooltip("Player's Rigidbody")]
    [SerializeField] new Rigidbody2D rigidbody;
    [Tooltip("Collider that checks if the player is touching the ground layer")]
    [SerializeField] BoxCollider2D groundCheck;
    [Tooltip("The Ground Layer Mask")]
    [SerializeField] LayerMask groudLayerMask;

    [Tooltip("Is the player facing to the right")]
    [SerializeField] bool facingRight = true;

    [Header("Movement Specific Variables")]
    [Tooltip("How fast is the player moving")]
    [Range(0f, 100f)] [SerializeField] float speed = 10f;
    [Range(0f, 10f)] [SerializeField] float gravityScale = 1f;

    [Header("Other Core Variables")]
    [Tooltip("The amount of health the player has")]
    [Range(0f, 100f)] [SerializeField] float health = 100;

    [Tooltip("The amount of health the player can have (MAX)")]
    [Range(0f, 100f)] [SerializeField] float maxHealth = 100;

    [Header("UI Variables")]
    [Tooltip("The healthbar of the player")]
    [SerializeField] Slider healthbar;

    private void Awake()
    {
        // If the rigidbody is not initialized, try to find it in the current GameObject
        if (rigidbody == null)
        {
            rigidbody = transform.GetComponent<Rigidbody2D>();
        }
        rigidbody.gravityScale = gravityScale;

        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck").GetComponent<BoxCollider2D>();
        }

        // Check for mistakes in initialization
        // Player's health can't be more than the maximum possible health
        // If this happens, exchange the values
        if (health > maxHealth)
        {
            float temp = health;
            health = maxHealth;
            maxHealth = temp;
        }

        if (healthbar == null)
        {
            healthbar = GameObject.Find("HealthBar").GetComponent<Slider>();
        }
    }

    private void Update()
    {
        // Update the UI
        healthbar.value = health / maxHealth;

        rigidbody.gravityScale = gravityScale;

    }

    // Move player with the default speed
    public void MovePlayer(Vector2 direction, bool jumping)
    {
        MovePlayer(direction, speed, jumping);
    }

    // Move player with custom speed
    public void MovePlayer(Vector2 direction, float speed, bool jumping)
    {
        // If player is moving to the left, flip them to face to the left
        if (direction.x < 0 && facingRight)
        {
            facingRight = false;
            transform.Rotate(0, -180, 0);
        }
        // If player is moving to the right, flip them to face to the right
        else if (direction.x > 0 && !facingRight)
        {
            facingRight = true;
            transform.Rotate(0, 180, 0);
        }

        //Debug.Log("Moving 2");
        rigidbody.velocity = new Vector2(direction.x * speed, rigidbody.velocity.y);
        // If the player is touching the ground, they can jump
        if (jumping && groundCheck.IsTouchingLayers(groudLayerMask)) rigidbody.velocity = new Vector2(rigidbody.velocity.x, speed);
    }

    public void TakeDamage(int amount)
    {
        // Function that manages health
    }

    //public GenericSaveData Save()
    //{
    //    // Save Position
    //    // Save Health
    //    // Save Ammo
    //    GenericSaveData saveData = new GenericSaveData(
    //        health,
    //        transform.GetComponentInChildren<WeaponBehaviour>().GetAmmo(),
    //        transform.position);

    //    return saveData;
    //}

    public void Load(SerializablePlayer data)
    {
        health = data.health;
        maxHealth = data.maxHealth;
        speed = data.speed;
        transform.GetComponentInChildren<WeaponBehaviour>().SetAmmo(data.ammo);
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
    }

    // Getters
    public float GetSpeed()
    {
        return speed;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int GetAmmo()
    {
        return transform.GetComponentInChildren<WeaponBehaviour>().GetAmmo();
    }
}

[System.Serializable]
public class SerializablePlayer
{
    public float speed;
    public float health;
    public float maxHealth;
    public float[] position;
    public int ammo;

    public SerializablePlayer(PlayerController player)
    {
        speed = player.GetSpeed();

        health = player.GetHealth();

        maxHealth = player.GetMaxHealth();

        position = new float[3];
        Vector3 v3Pos = player.GetPosition();
        position[0] = v3Pos.x;
        position[1] = v3Pos.y;
        position[2] = v3Pos.z;

        ammo = player.GetAmmo();

    }
}