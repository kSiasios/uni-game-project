using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    new ParticleSystem particleSystem;
    [Tooltip("The damage that the particle inflicts on impact")]
    [SerializeField] int bulletDamage = 10;
    [Tooltip("The speed of the particle")]
    [Range(5f, 100f)] public float bulletSpeed = 20f;

    ParticleSystem.MainModule _particleMainModule;

    private void Awake()
    {
        particleSystem = transform.GetComponent<ParticleSystem>();
        _particleMainModule = particleSystem.main;
        _particleMainModule.startSpeed = bulletSpeed;
    }

    // Function that handles the collision of the particle,
    // damage the enemy when collided with
    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyBehaviour enemy;
            other.transform.TryGetComponent(out enemy);
            enemy.TakeDamage(bulletDamage);
            Debug.Log("Enemy Hit!");
        }
        //Destroy(this);
    }
}
