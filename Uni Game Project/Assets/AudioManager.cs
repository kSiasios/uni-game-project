using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlaySound(string soundName)
    {
        //Debug.Log("Playing sound: " + soundName);
        //audioSource.Stop();
        audioSource.Play();
    }
}
