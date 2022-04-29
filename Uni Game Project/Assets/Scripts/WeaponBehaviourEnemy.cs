using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviourEnemy : MonoBehaviour
{

    [Header("Weapon Attributes")]
    [Tooltip("The amount of bullets the weapon can hold")]
    [SerializeField] int magazineSize = 12;
    [Tooltip("The reload speed of the weapon in seconds")]
    [SerializeField] float reloadSpeed = 5f;
    [Tooltip("The amount of damage the weapon can deal")]
    [SerializeField][Range(0, 100)] float damage = 50;
    [Tooltip("The amount of bullets fired at once")]
    [SerializeField][Range(1, 5)] int bulletsFired = 1;
    [Tooltip("Time between shots")]
    [SerializeField] float delayAfterShooting = 5f;

    [Header("Required References")]
    [Tooltip("The controller of this weapon")]
    [SerializeField] EnemyBehaviour controller;
    [Tooltip("Where the bullets are fired from")]
    [SerializeField] Transform shootingPoint;
    [Tooltip("The patricle system that spawns bullets")]
    [SerializeField] ParticleSystem bulletParticleSystem;

    [SerializeField][Range(0, 100)] public float bulletSpeed = 10f;
    [SerializeField] int _bulletsFiredSoFar = 0;
    [SerializeField] bool _reloading = false;

    Coroutine reloadCoroutine;
    Coroutine shootCoroutine;

    ParticleSystem.MainModule _mainModule;
    ParticleSystem.EmissionModule _emissionModule;

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

        _mainModule = bulletParticleSystem.main;
        _mainModule.duration = delayAfterShooting;

        _emissionModule = bulletParticleSystem.emission;
        _emissionModule.rateOverTime = 1 / delayAfterShooting;
    }

    // Update is called once per frame
    void Update()
    {
        //if (controller.chasingPlayer)
        //{
        //    ShootWeapon();
        //}

        if (controller.chasingPlayer)
        {
            if (!_reloading)
            {
                // If the bullets available are enough to shoot, do it. Else, reload the weapon.
                if (_bulletsFiredSoFar < magazineSize)
                {
                    ShootWeapon();
                }
                else
                {
                    //if (shootCoroutine != null)
                    //{
                    //    StopCoroutine(shootCoroutine);
                    //}
                    ReloadWeapon();
                }
            }
        }

        //if (_bulletsFiredSoFar >= magazineSize)
        //{
        //    if (!_reloading)
        //    {
        //        reloadCoroutine =  StartCoroutine(Reload());
        //    }
        //} else
        //{
        //    if (reloadCoroutine != null)
        //    {
        //        StopCoroutine(reloadCoroutine);
        //    }
        //}
    }

    public void ShootWeapon()
    {
        //IEnumerator coroutine = Shoot();
        //if (_bulletsFiredSoFar <= magazineSize)
        //{
        //    //totalAmmo -= (magazineSize - bulletsIn?agazine);
        //    //bulletsIn?agazine = magazineSize;
        //    StartCoroutine(coroutine);
        //}
        //else
        //{
        //    Debug.Log("Magazine Empty!");
        //    StopCoroutine(coroutine);
        //}

        if (!bulletParticleSystem.isEmitting && !_reloading)
        {
            bulletParticleSystem.Emit(bulletsFired);
            bulletParticleSystem.Play();
            //_bulletsFiredSoFar += Mathf.RoundToInt(_mainModule.startLifetime.constant / delayAfterShooting);
            _bulletsFiredSoFar++;
        }

        //if (bulletParticleSystem.isPlaying)
        //{
        //    Debug.Log("PLAYING");
        //}
        //if (bulletParticleSystem.isEmitting)
        //{
        //    //bulletParticleSystem.time
        //    //Debug.Log("Time: " + bulletParticleSystem.time);
        //    if (bulletParticleSystem.time >= delayAfterShooting)
        //    {
        //        bulletParticleSystem.Stop();
        //        //_bulletsFiredSoFar++;
        //    }
        //}
    }

    public void ReloadWeapon()
    {
        // stop shooting
        bulletParticleSystem.Stop();

        IEnumerator coroutine = Reload();
        if (_bulletsFiredSoFar > 0)
        {
            //totalAmmo -= (magazineSize - bulletsIn?agazine);
            //bulletsIn?agazine = magazineSize;
            StartCoroutine(coroutine);
        }
        else
        {
            Debug.Log("Magazine Full!");
            StopCoroutine(coroutine);
        }
    }

    //IEnumerator Shoot()
    //{
    //    //while (_bulletsFiredSoFar < magazineSize)
    //    //{
    //    //    _bulletsFiredSoFar += bulletsFired;

    //    //    bulletParticleSystem.Emit(bulletsFired);
    //    //    bulletParticleSystem.Play();
    //    //        Debug.Log("Bullets fired: " + _bulletsFiredSoFar);
    //    //    yield return new WaitForSeconds(delayAfterShooting);
    //    //}

    //    for (int i = 0; i < magazineSize; i++)
    //    {
    //        //for (int j = 0; j <= i; j++)
    //        //{
    //        //    Debug.Log(printMsg[j]);
    //        //}

    //        // Create new dialog item object
    //        //DialogItem dialogItem = new DialogItem(printMsg.Substring(0, i + 1), currentSpeaker.Image);

    //        ////Debug.Log(dialogItem);

    //        //// Populate dialog system
    //        //WriteDialog(dialogItem);

    //        //Debug.Log(printMsg.Substring(0, i + 1));
    //        //yield return new WaitForSeconds(printDelay);

    //        bulletParticleSystem.Emit(bulletsFired);
    //            bulletParticleSystem.Play();
    //        Debug.Log("Bullets fired: " + _bulletsFiredSoFar);
    //        _bulletsFiredSoFar = i;

    //        yield return new WaitForSeconds(delayAfterShooting);
    //    }
    //}

    public IEnumerator Reload()
    {

        while (_bulletsFiredSoFar > 0)
        {
            //Debug.Log("RELOADING");
            _reloading = true;
            yield return new WaitForSeconds(reloadSpeed);
            if (_bulletsFiredSoFar <= 0)
            {
                _reloading = false;
                break;
            }
            _bulletsFiredSoFar--;
        }
        _reloading = false;
    }
}
