using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
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
