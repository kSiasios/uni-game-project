using System.Collections;
using UnityEngine;

public class EnviromentalAudioSource : AudioSourceController
{
    private void Awake()
    {
        shouldDestroyExtraAudioSources = false;
        base.Awake();  
    }

    void Start()
    {
        //StartCoroutine(FadeAudioIn());
        //FadeIn(null, timeToFade);
        base.Start();
    }
}
