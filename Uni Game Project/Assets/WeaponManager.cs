using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBehaviour[] weaponsList;
    [SerializeField] private EquipedWeapon equippedWeapon = EquipedWeapon.none;
    [SerializeField] private PlayerController player;

    [SerializeField] private GameObject regularArm;
    [SerializeField] private GameObject armHoldingWeapon;

    [Header("UI References")]
    [Tooltip("The UI field that displays the amount of bullets")]
    [SerializeField] TextMeshProUGUI bulletsAmountUI;

    enum EquipedWeapon
    {
        none = 0, shotgun, pistol, bat
    }

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        if (weaponsList.Length == 0)
        {
            Debug.LogError("WEAPONS LIST IS EMPTY!");
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if (armHoldingWeapon == null)
        {
            Debug.LogError("ARM HOLDING WEAPON IS NULL!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (regularArm == null)
        {
            Debug.LogError("REGULAR ARM IS NULL!");
            UnityEditor.EditorApplication.isPlaying = false;
        }


        if (bulletsAmountUI == null)
        {
            bulletsAmountUI = FindObjectOfType<Canvas>().transform.Find("GameplayUI").transform.Find("AmmoCounter").transform.Find("Item").transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
        }

        DeactivateAllWeapons();
        if (equippedWeapon != EquipedWeapon.none)
        {
            ActivateWeapon((int)equippedWeapon);
        } else
        {
            bulletsAmountUI.text = "-";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RotateWeapons();
        }

        // If player's input is translated to in-game input
        if (GameManager.canGetGameplayInput)
        {
            if (equippedWeapon != EquipedWeapon.none)
            {
                // If the fire button is pressed
                //if (Input.GetButtonDown("Fire1"))
                //{
                //    //weaponsList[(int)equippedWeapon - 1].Fire();
                //    FireWeapon();
                //}

                // If the reload key is pressed, reload the weapon
                if (Input.GetKeyDown(KeyCode.R))
                {
                    weaponsList[(int)equippedWeapon - 1].ReloadWeapon();
                }
            }
        }
    }

    private void RotateWeapons()
    {
        if ((int)equippedWeapon < weaponsList.Length)
        {
            equippedWeapon++;
            DeactivateAllWeapons();
            ActivateWeapon((int)equippedWeapon);
        }
        else
        {
            equippedWeapon = 0;
            //regularArm.SetActive(true);
            //armHoldingWeapon.SetActive(false);
            DeactivateAllWeapons();

            bulletsAmountUI.text = "-";
        }
        Debug.Log("Equipped Weapon: " + equippedWeapon);
    }

    private void DeactivateAllWeapons()
    {
        for (int i = 0; i < weaponsList.Length; i++)
        {
            weaponsList[i].gameObject.SetActive(false);
        }
        armHoldingWeapon.SetActive(false);
        regularArm.SetActive(true);
    }

    private void ActivateWeapon(int weaponIndex)
    {
        regularArm.SetActive(false);
        armHoldingWeapon.SetActive(true);
        weaponsList[weaponIndex - 1].gameObject.SetActive(true);
    }

    public void FireWeapon()
    {
        //Debug.Log("GameManager.canGetGameplayInput: " + GameManager.canGetGameplayInput);
        if (GameManager.canGetGameplayInput)
        {
            if ((int)equippedWeapon > 0)
            {
                weaponsList[(int)equippedWeapon - 1].Fire();
            }
        }
    }
}
