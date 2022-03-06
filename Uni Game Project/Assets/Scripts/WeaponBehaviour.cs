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
    [Range(0, 100)] [SerializeField] float damage = 50;
    [Tooltip("The amount of bullets fired at once")]
    [Range(1, 5)] [SerializeField] int bulletsFired = 1;

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

        if (bulletsAmountUI == null)
        {
            bulletsAmountUI = GameObject.Find("AmmoCounter").GetComponentInChildren<TextMeshProUGUI>();
            totalAmmo -= bulletsInΜagazine;
            bulletsAmountUI.text = bulletsInΜagazine + " / " + totalAmmo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If player's input is translated to in-game input
        if (GameManager.canGetGameplayInput)
        {
            // If the fire button is pressed
            if (Input.GetButtonDown("Fire1"))
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
                // Update the UI
                bulletsAmountUI.text = bulletsInΜagazine.ToString() + " / " + totalAmmo.ToString();
            }

            // If the reload key is pressed, reload the weapon
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadWeapon();
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Fire");

        // Create an objects that holds the parameters of the emitter
        var emitParams = new ParticleSystem.EmitParams();

        // Reduce ammo
        bulletsInΜagazine -= bulletsFired;

        bulletParticleSystem.Emit(emitParams, bulletsFired);
        bulletParticleSystem.Play();
    }

    void ReloadWeapon()
    {
        IEnumerator coroutine;
        if (bulletsInΜagazine < magazineSize)
        {
            //totalAmmo -= (magazineSize - bulletsInΜagazine);
            //bulletsInΜagazine = magazineSize;
            coroutine = Reload();
            StartCoroutine(coroutine);
        }
        else
        {
            Debug.Log("Magazine Full!");
        }
    }

    private IEnumerator Reload()
    {
        while (bulletsInΜagazine < magazineSize)
        {
            yield return new WaitForSeconds(reloadSpeed);
            totalAmmo -= 1;
            bulletsInΜagazine++;
            // Update the UI
            bulletsAmountUI.text = bulletsInΜagazine.ToString() + " / " + totalAmmo.ToString();
            Debug.Log("Reloading " + bulletsInΜagazine);
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
        bulletsAmountUI.text = bulletsInΜagazine.ToString() + " / " + totalAmmo.ToString();
    }
}
