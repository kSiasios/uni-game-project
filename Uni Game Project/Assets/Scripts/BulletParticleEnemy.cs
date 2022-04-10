using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleEnemy : MonoBehaviour
{
    new ParticleSystem particleSystem;
    [Tooltip("The damage that the particle inflicts on impact")]
    [SerializeField] int bulletDamage = 10;
    [Tooltip("The speed of the particle")]
    [Range(5f, 50f)] public float bulletSpeed = 20f;
    private void Awake()
    {
        particleSystem = transform.GetComponent<ParticleSystem>();
    }

    // Function that handles the collision of the particle,
    // damage the player when collided with
    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController player;
            other.transform.TryGetComponent(out player);
            player.TakeDamage(bulletDamage);
            Debug.Log("Player Hit!");
        }
        //Destroy(this);
    }
}
