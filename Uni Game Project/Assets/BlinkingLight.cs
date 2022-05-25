using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : FlickeringLight
{
    [SerializeField][Range(0f, 20f)] float blinkPeriod = 2f;
    [SerializeField] float timer = 0f;

    private void Awake()
    {
        base.Awake();
        flickerPosibility = 100;
        flickerAmount = 100;
        affectRadius = false;
    }

    new void Flicker()
    {
        StartCoroutine(Blink());
    }

    void FixedUpdate()
    {
        //base.FixedUpdate();
        Flicker();
    }

    IEnumerator Blink()
    {
        if (timer < blinkPeriod)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (timer >= blinkPeriod)
        {
            if (light.intensity == 0)
            {
                light.intensity = defaultIntensity;
            }else 
            if (light.intensity != 0)
            {
                light.intensity = 0f;
            }
            timer = 0f;
        }
        yield break;
        //yield return null;
    }
}
