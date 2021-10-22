using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{

    [Header("Weapon Attributes")]
    [Tooltip("The amount of bullets the weapon can hold")]
    [SerializeField] int magazineSize = 12;
    [Tooltip("The reload speed of the weapon in seconds")]
    [SerializeField] float reloadSpeed = 0.3f;
    [Tooltip("The amount of damage the weapon can deal")]
    [Range(0, 100)][SerializeField] float damage = 50;
    [Range(1, 5)][SerializeField] int bulletsFired = 1;

    [Header("Required References")]
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform shootingPoint;
    [SerializeField] ParticleSystem bulletParticleSystem;

    private void Awake()
    {
        // If playerController is not initialized in the inspector, get it using code
        if (playerController == null)
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }

        if (shootingPoint == null)
        {
            shootingPoint = transform.Find("ShootingPoint").transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("Fire");
        //bulletParticleSystem.
        //bulletParticleSystem.transform.SetPositionAndRotation(shootingPoint.position, bulletParticleSystem.transform.rotation);
        bulletParticleSystem.Emit(bulletsFired);
        bulletParticleSystem.Play();
    }
}
