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
    [SerializeField] GroundCheck groundCheck;
    [Tooltip("The Ground Layer Mask")]
    static string[] layerMaskLayers = { "Ground", "Metal", "Wood" };
    public static LayerMask groundLayerMask;
    //= LayerMask.GetMask(layerMaskLayers);

    [Tooltip("Is the player facing to the right")]
    [SerializeField] bool facingRight = true;

    [Header("Movement Specific Variables")]
    [Tooltip("How fast is the player moving")]
    [Range(0f, 100f)][SerializeField] float speed = 10f;
    [Range(0f, 100f)][SerializeField] float jumpForce = 10f;
    [Range(0f, 100f)][SerializeField] float gravityScale = 1f;

    [Header("Other Core Variables")]
    [Tooltip("The amount of health the player has")]
    [Range(0f, 100f)][SerializeField] float health = 100;

    [Tooltip("The amount of health the player can have (MAX)")]
    [Range(0f, 100f)][SerializeField] float maxHealth = 100;

    [Header("UI Variables")]
    [Tooltip("The healthbar of the player")]
    [SerializeField] Slider healthbar;

    [Header("Debugging Variables")]
    [SerializeField] private bool canReset = true;
    [SerializeField] private Vector3 customResetPosition;
    [SerializeField] private Vector3 resetPosition;

    [Header("Animation Variables")]
    [Tooltip("The animator responsible for the player")]
    [SerializeField] Animator playerAnimator;

    [Header("DEBUG ONLY")]
    [Tooltip("On debug mode?")]
    [SerializeField] bool debug;
    [Tooltip("Debug Mode speed?")]
    [SerializeField] float debugModeSpeed = 100f;
    [Tooltip("Debug Mode gravity scale?")]
    [SerializeField] float debugModeGravityScale = 10f;


    private bool isJumping = false;

    private WeaponManager weaponManager;

    private AudioManager audioManager;

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask(layerMaskLayers);
        // Initialize reset position if the player can reset
        if (canReset)
        {
            if (resetPosition == customResetPosition)
            {
                // This means that the two positions are uninitialized or the user put 0,0,0 as the reset position
                resetPosition = transform.position;
            }
            else
            {
                resetPosition = customResetPosition;
            }
        }

        // If the rigidbody is not initialized, try to find it in the current GameObject
        if (rigidbody == null)
        {
            rigidbody = transform.GetComponent<Rigidbody2D>();
        }
        rigidbody.gravityScale = gravityScale;

        if (groundCheck == null)
        {
            groundCheck = GetComponentInChildren<GroundCheck>();
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

        if (playerAnimator == null)
        {
            playerAnimator = gameObject.GetComponent<Animator>();
        }

        weaponManager = GetComponent<WeaponManager>();
        audioManager = GetComponent<AudioManager>();

        SetDebug(debug);
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
        isJumping = jumping;
        SetAnimatorBool("isJumping", jumping);
        MovePlayer(direction, speed, jumping);
    }

    // Move player with custom speed
    public void MovePlayer(Vector2 direction, float speed, bool jumping)
    {
        //if (direction == Vector2.zero && rigidbody.velocity != Vector2.zero && !jumping && groundCheck.IsTouchingLayers(groundLayerMask))
        //{
        //    Debug.Log("Sliding off of slope, or moved against will");
        //    //rigidbody.velocity = Vector2.zero;
        //    //rigidbody.gravityScale = 0;
        //    rigidbody.bodyType = RigidbodyType2D.Static;
        //} else
        //{
        //    //rigidbody.gravityScale = gravityScale;
        //    rigidbody.bodyType = RigidbodyType2D.Dynamic;
        //}
        // If player is moving to the left, flip them to face to the left
        if (direction.x < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        // If player is moving to the right, flip them to face to the right
        else if (direction.x > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        //Debug.Log("Moving 2");
        rigidbody.velocity = new Vector2(direction.x * speed, rigidbody.velocity.y);
        // If the player is touching the ground, they can jump
        if (jumping && groundCheck.GetIsTouching(groundLayerMask)) rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);

        // If the player is landing, play landing animation
        if (groundCheck.GetIsTouching(groundLayerMask))
        {
            if (rigidbody.velocity.y < -1)
            {
                SetAnimatorBool("isLanding", true);
            }
            else
            {
                // Play falling animation
                SetAnimatorBool("isLanding", false);
                //SetAnimatorBool("isJumping", true);
            }
        }
        else
        {
            SetAnimatorBool("isLanding", false);
            SetAnimatorBool("isJumping", false);
        }

        if (Mathf.Abs(rigidbody.velocity.x) > 4)
        {
            // Is running
            SetAnimatorBool("isRunning", true);
            SetAnimatorBool("isMoving", false);
        }
        else if (Mathf.Abs(rigidbody.velocity.x) > 0.1)
        {
            SetAnimatorBool("isRunning", false);
            SetAnimatorBool("isMoving", true);
            //playerAnimator.SetBool("isRunning", false);
        }
        else
        {
            SetAnimatorBool("isRunning", false);
            SetAnimatorBool("isMoving", false);
        }
    }

    public void TakeDamage(int amount)
    {
        // Function that manages health
        //Debug.Log("DAMAGED");
        health -= amount;
        Debug.Log($"Current Health: {health}");
        audioManager.PlayerHit();

        if (health <= 0)
        {
            // DEATH
            // freeze time
            Time.timeScale = 0f;
            // begin death sequence
            FindObjectOfType<GameManager>(true).DeathSequence();
        }
    }

    public void Land()
    {
        SetAnimatorBool("isLanding", false);
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
        WeaponBehaviour weapon = FindObjectOfType<WeaponBehaviour>();
        if (weapon != null)
        {
            weapon.SetAmmo(data.ammo);
        }
        //transform.GetComponentInChildren<WeaponBehaviour>().SetAmmo(data.ammo);
        //transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        //transform.position = new Vector3(data.position.x, data.position.y, data.position.z);
        if (data.parentName != "")
        {
            GameObject parentGameObject = GameManager.FindInActiveObjectByName(data.parentName);
            //Debug.Log($"Trying to find: {data.parentName}, Found: {parentGameObject}");
            parentGameObject.SetActive(true);
            transform.parent = parentGameObject.transform;
        }

        //transform.position = new Vector3(data.positionX, data.positionY + 5f, data.positionZ);
        transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
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
        WeaponBehaviour weaponBehaviour = FindObjectOfType<WeaponBehaviour>();
        if (weaponBehaviour != null)
        {
            return weaponBehaviour.GetAmmo();
        }
        return 999;
    }

    public bool GetIfJumping()
    {
        return isJumping;
    }

    public static LayerMask GetGroundLayerMask()
    {
        return groundLayerMask;
    }

    //public void stopPlayer()
    //{
    //    if(groundCheck.IsTouchingLayers(groundLayerMask) && rigidbody.velocity != Vector2.zero)
    //    {
    //        // IF PLAYER IS GROUNDED, THEN PREVENT THEM FROM SLIDING
    //        rigidbody.constraints = UnityEngine.RigidbodyConstraints2D.FreezePositionX;
    //    }
    //}.

    public void FireWeapon()
    {
        weaponManager.FireWeapon();
    }

    public void Reset()
    {
        // A function that resets the player's position (for debugging or potential checkpoints)
        transform.position = resetPosition;
        rigidbody.velocity = Vector2.zero;
    }
    // Setters
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetAnimatorBool(string varName, bool value)
    {
        playerAnimator.SetBool(varName, value);
    }

    public void SetStartOfRunAnimation(string value)
    {
        if (value.ToLower() == "true")
        {
            SetAnimatorBool("canStartRun", true);
        }
        else
        {
            SetAnimatorBool("canStartRun", false);
        }
    }

    public void SetDebug(bool enabled)
    {
        if (enabled)
        {
            speed = debugModeSpeed;
            rigidbody.gravityScale = debugModeGravityScale;
        }
    }
}

[System.Serializable]
public class SerializablePlayer
{
    public float speed;
    public float health;
    public float maxHealth;
    //public float[] position;
    public float positionX, positionY, positionZ;
    //public SerializableVector3 position;
    public int ammo;
    public string parentName;

    public SerializablePlayer(PlayerController player)
    {
        Debug.Log($"Serialize data for player controller: {player}");
        speed = player.GetSpeed();

        health = player.GetHealth();

        maxHealth = player.GetMaxHealth();

        //position = new float[3];
        Vector3 v3Pos = player.GetPosition();
        //position[0] = v3Pos.x;
        //position[1] = v3Pos.y;
        //position[2] = v3Pos.z;
        positionX = v3Pos.x;
        positionY = v3Pos.y;
        positionZ = v3Pos.z;

        ammo = player.GetAmmo();

        string tempParentName = "";

        if (player.transform.parent != null)
        {
            tempParentName = player.transform.parent.name;
        }
        parentName = tempParentName;

    }

    public SerializablePlayer(float speed, float health, float maxHealth, Vector3 position, int ammo, string parentName)
    {
        //Debug.Log($"Serialize data for player controller: {player}");
        this.speed = speed;

        this.health = health;

        this.maxHealth = maxHealth;

        //this.position = new float[3];
        Vector3 v3Pos = position;
        //position[0] = v3Pos.x;
        //position[1] = v3Pos.y;
        //position[2] = v3Pos.z;
        //position.x = v3Pos.x;
        //position.y = v3Pos.y;
        //position.z = v3Pos.z;
        positionX = v3Pos.x;
        positionY = v3Pos.y;
        positionZ = v3Pos.z;

        this.ammo = ammo;

        this.parentName = parentName;

        //Debug.Log($"Player Created: Position -> {position}");
    }
}