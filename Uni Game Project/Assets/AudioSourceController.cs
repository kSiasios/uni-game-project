using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    [Tooltip("The volume at which the environmental sounds play.")]
    [SerializeField] private float volume;
    [Tooltip("The time that takes for the volume to reach its maximum (or zero) when fading.")]
    [SerializeField] protected float timeToFade;
    protected AudioSource audioSource;

    protected bool shouldDestroyExtraAudioSources = true;

    protected bool canFadeIn = true;

    public float Volume { get => volume; set => volume = value; }

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;

        //coroutine = StartCoroutine(FadeAudioIn());
    }

    protected void Start()
    {
        //StartCoroutine(FadeAudioIn());
        FadeIn(null, timeToFade);
    }

    protected IEnumerator FadeAudioIn(AudioClip clip)
    {
        while (!canFadeIn)
        {
            //Debug.Log("CANNOT FADE IN");
            // CANNOT FADE IN
            yield return null;
        }
        if (clip != null)
        {
            audioSource.clip = clip;
        }
        audioSource.Play();
        float currentTime = 0;
        //float start = audioSource.volume;
        float start = 0;
        while (currentTime < timeToFade)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, volume, currentTime / timeToFade);
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeAudioOut()
    {
        canFadeIn = false;
        float currentTime = 0;
        float start = audioSource.volume;
        //float start = 0;
        while (currentTime < timeToFade)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / timeToFade);
            yield return null;
        }
        canFadeIn = true;
        yield break;
    }

    public void FadeIn(AudioClip clip, float overTime)
    {
        if (overTime != 0f)
        {
            timeToFade = overTime;
        }
        StartCoroutine(FadeAudioIn(clip));
    }

    public void FadeOut(float overTime)
    {
        if (overTime != 0f)
        {
            timeToFade = overTime;
        }
        StartCoroutine(FadeAudioOut());
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    public float GetPitch()
    {
        return audioSource.pitch;
    }

    public AudioSource CreateAudioSource(AudioClip clip)
    {
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        //newAudioSource.clip = clip;
        //newAudioSource.Play();
        return newAudioSource;
    }

    private void FixedUpdate()
    {
        if (shouldDestroyExtraAudioSources)
        {
            AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();

            foreach (var audioSource in audioSources)
            {
                if (!audioSource.isPlaying)
                {
                    Destroy(audioSource);
                }
            }
        }
    }

    public bool IsPlaying()
    {
        if (audioSource.volume <= 0.01f)
        {
            return false;
        }
        return true;
    }
}
