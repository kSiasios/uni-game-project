using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    new ParticleSystem particleSystem;
    [SerializeField] int bulletDamage = 10;
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
