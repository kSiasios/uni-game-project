using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviourEnemy : MonoBehaviour
{

    [Header("Weapon Attributes")]
    [Tooltip("The amount of bullets the weapon can hold")]
    [SerializeField] int magazineSize = 12;
    [Tooltip("The reload speed of the weapon in seconds")]
    [SerializeField] float reloadSpeed = 0.3f;
    [Tooltip("The amount of damage the weapon can deal")]
    [Range(0, 100)] [SerializeField] float damage = 50;
    [Tooltip("The amount of bullets fired at once")]
    [Range(1, 5)] [SerializeField] int bulletsFired = 1;

    [Header("Required References")]
    [Tooltip("The controller of this weapon")]
    [SerializeField] EnemyBehaviour controller;
    [Tooltip("Where the bullets are fired from")]
    [SerializeField] Transform shootingPoint;
    [Tooltip("The patricle system that spawns bullets")]
    [SerializeField] ParticleSystem bulletParticleSystem;

    private void Awake()
    {
        // If controller is not initialized in the inspector, get it using code
        if (controller == null)
        {
            controller = transform.parent.GetComponent<EnemyBehaviour>();
        }

        if (shootingPoint == null)
        {
            shootingPoint = transform.Find("ShootingPoint").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.chasingPlayer)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //Debug.Log("Fire");
        // Create an objects that holds the parameters of the emitter
        var emitParams = new ParticleSystem.EmitParams();

        // Populate the parameters object according to the type of the controller
        if (controller.enemyType == EnemyBehaviour.EnemyType.Flyer)
        {
            if (bulletParticleSystem.name == "Bomb")
            {
                // The grenade will free fall from the position of the entity
                emitParams.velocity = new Vector3(0, 0, 0);
            } else
            {
                // The bullet will go straight below the entity with a speed of bulletSpeed
                emitParams.velocity = new Vector3(0 ,-bulletParticleSystem.transform.GetComponent<BulletParticleEnemy>().bulletSpeed, 0);
            }
        }

        // Spawn particles
        bulletParticleSystem.Emit(emitParams, bulletsFired);
        bulletParticleSystem.Play();
        bulletParticleSystem.Stop();
    }
}
