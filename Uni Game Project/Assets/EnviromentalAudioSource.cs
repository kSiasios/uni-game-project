using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentalAudioSource : MonoBehaviour
{
    [Tooltip("The volume at which the environmental sounds play.")]
    [SerializeField] float volume;
    [Tooltip("The time that takes for the volume to reach its maximum (or zero) when fading.")]
    [SerializeField] float timeToFade;
    AudioSource audioSource;


    Coroutine coroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;

        coroutine = StartCoroutine(FadeAudioIn());
    }

    void Start()
    {
        StartCoroutine(FadeAudioIn());
    }

    //IEnumerator DoSomething(float someParameter)
    //{
    //    while (true)
    //    {
    //        print("DoSomething Loop");

    //        // Yield execution of this coroutine and return to the main loop until next frame
    //        yield return null;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        //IEnumerator coroutine = FadeAudioIn();
        //if (audioSource.volume >= volume)
        //{
        //    //coroutine = StartCoroutine(FadeAudioIn());
        //    StopCoroutine(coroutine);
        //}
    }

    IEnumerator FadeAudioIn()
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < timeToFade)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, volume, currentTime / timeToFade);
            yield return null;
        }
        yield break;
    }
}
