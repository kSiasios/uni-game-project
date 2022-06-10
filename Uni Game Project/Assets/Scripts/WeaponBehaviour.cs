using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponBehaviour : MonoBehaviour
{

    [Header("Weapon Attributes")]
    [Tooltip("The amount of bullets held by the player")]
    [SerializeField] int totalAmmo = 999;
    [Tooltip("The amount of bullets the weapon can hold")]
    [SerializeField] int magazineSize = 12;
    [Tooltip("The amount of bullets the weapon is holding at the moment")]
    [SerializeField] int bulletsInΜagazine = 12;
    [Tooltip("The reload speed of the weapon in seconds")]
    [SerializeField] float reloadSpeed = 0.3f;
    [Tooltip("The amount of damage the weapon can deal")]
    [Range(0, 100)][SerializeField] float damage = 50;
    [Tooltip("The amount of bullets fired at once")]
    [Range(1, 5)][SerializeField] int bulletsFired = 1;
    [Tooltip("Should the player have infinite ammo?")]
    [SerializeField] bool infiniteAmmo = false;

    [Header("Required References")]
    [Tooltip("The controller of this weapon")]
    [SerializeField] PlayerController playerController;
    [Tooltip("Where the bullets are fired from")]
    [SerializeField] Transform shootingPoint;
    [Tooltip("The patricle system that spawns bullets")]
    [SerializeField] ParticleSystem bulletParticleSystem;

    [Header("UI References")]
    [Tooltip("The UI field that displays the amount of bullets")]
    [SerializeField] TextMeshProUGUI bulletsAmountUI;

    [Header("Sound")]
    [SerializeField] AudioClip shootingSound;
    [SerializeField] AudioClip reloadingSound;

    bool reloading = false;

    private void Awake()
    {
        // If playerController is not initialized in the inspector, get it using code
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            //playerController = GetComponent<PlayerController>();
        }

        if (shootingPoint == null)
        {
            shootingPoint = transform.Find("ShootingPoint").transform;
        }

        if (bulletsAmountUI == null)
        {
            bulletsAmountUI = GameObject.Find("AmmoCounter").GetComponentInChildren<TextMeshProUGUI>();
            totalAmmo -= bulletsInΜagazine;
            if (!infiniteAmmo)
            {
                bulletsAmountUI.text = $"{bulletsInΜagazine} / {totalAmmo}";
            }
        }
    }

    private void FixedUpdate()
    {
        // Purge AudioSources that don't play
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (var item in audioSources)
        {
            if (!item.isPlaying)
            {
                Destroy(item);
            }
        }
    }

    public void Fire()
    {
        if (!reloading)
        {

            // If the bullets available are enough to shoot, do it. Else, reload the weapon.
            if (bulletsInΜagazine >= bulletsFired)
            {
                Shoot();
            }
            else
            {
                ReloadWeapon();
            }
        }

        // Update the UI
        if (!infiniteAmmo)
        {
            bulletsAmountUI.text = $"{bulletsInΜagazine} / {totalAmmo}";
        }
    }

    void Shoot()
    {
        //Debug.Log("Fire");

        // Create an objects that holds the parameters of the emitter
        //var emitParams = new ParticleSystem.EmitParams();

        // Reduce ammo
        bulletsInΜagazine -= bulletsFired;

        //bulletParticleSystem.Emit(emitParams, bulletsFired);
        //bulletParticleSystem.Play();
        //bulletParticleSystem.Emit();
        bulletParticleSystem.Emit(bulletsFired);
        bulletParticleSystem.Play();

        // Play sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(shootingSound);
    }

    public void ReloadWeapon()
    {
        IEnumerator coroutine = Reload();
        // Play sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(reloadingSound);
        if (bulletsInΜagazine < magazineSize)
        {
            //totalAmmo -= (magazineSize - bulletsInΜagazine);
            //bulletsInΜagazine = magazineSize;
            StartCoroutine(coroutine);
        }
        else
        {
            Debug.Log("Magazine Full!");
            StopCoroutine(coroutine);
        }
    }

    private IEnumerator Reload()
    {
        while (bulletsInΜagazine < magazineSize)
        {
            reloading = true;
            WeaponManager.Reloading = reloading;
            yield return new WaitForSeconds(reloadSpeed);
            if (bulletsInΜagazine >= magazineSize)
            {
                reloading = false;
                break;
            }
            if (!infiniteAmmo)
            {
                totalAmmo -= 1;
                // Update the UI
                bulletsAmountUI.text = $"{bulletsInΜagazine} / {totalAmmo}";
            }
            bulletsInΜagazine++;
            //Debug.Log("Reloading " + bulletsInΜagazine);
        }


        if (bulletsInΜagazine >= magazineSize)
        {
            reloading = false;
            WeaponManager.Reloading = reloading;
        }
    }

    public int GetAmmo()
    {
        return totalAmmo;
    }

    public void SetAmmo(int value)
    {
        totalAmmo = value;
        RefreshUI();
    }

    void RefreshUI()
    {
        bulletsAmountUI.text = $"{bulletsInΜagazine} / {totalAmmo}";
    }
}
