using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    [SerializeField] AudioClip clickSound;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void PlayHover()
    {
        audioSource.PlayOneShot(hoverSound);
    }
}
