using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] CustomAudioClip clipToPlay;
    [SerializeField] bool isEnvironmental = false;

    //[SerializeField] AudioSource audioSource
    [SerializeField] EnviromentalAudioSource enviromentalAudioSource;
    //[SerializeField] AudioSource audioSource;

    [SerializeField][Range(0f, 60f)] float fadeInTime = 0f;
    [SerializeField][Range(0f, 60f)] float fadeOutTime = 0f;


    private void Awake()
    {
        if (clipToPlay == null)
        {
            Debug.LogError("Clip to play was NOT provided!");
        }

        if (isEnvironmental && enviromentalAudioSource == null)
        {
            enviromentalAudioSource = FindObjectOfType<EnviromentalAudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            // fade audio in
            if (isEnvironmental)
            {
                //Debug.Log("FADE OUT");
                enviromentalAudioSource.Volume = clipToPlay.Volume;

                enviromentalAudioSource.FadeOut(fadeOutTime);
                //Debug.Log("FADE IN");
                enviromentalAudioSource.FadeIn(clipToPlay.AudioClip, fadeInTime);
            }
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.GetComponent<PlayerController>())
    //    {
    //        // fade audio out
    //        if (isEnvironmental)
    //        {
    //            Debug.Log("FADE OUT");
    //            //enviromentalAudioSource.FadeOut(fadeOutTime);
    //        }
    //    }
    //}
}
