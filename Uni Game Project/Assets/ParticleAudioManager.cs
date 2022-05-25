using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioManager : MonoBehaviour
{
    [SerializeField] CustomAudioClip clipOnEmission;

    [SerializeField] AudioSource audioSource;

    private int particlesCount = 0;

    [SerializeField] ParticleSystem pSystem;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (pSystem == null)
        {
            pSystem = GetComponent<ParticleSystem>();
        }
    }

    private void Update()
    {
        if (particlesCount < pSystem.particleCount)
        {
            // we emitted => play sound
            clipOnEmission.PlayClip(gameObject);
        }

        particlesCount = pSystem.particleCount;

        foreach (var audioSource in GetComponents<AudioSource>())
        {
            if (audioSource.volume == 0 || !audioSource.isPlaying)
            {
                Destroy(audioSource);
            }
        }
    }
}
