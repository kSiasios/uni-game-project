using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationItem : MonoBehaviour
{
    [SerializeField] float durationTime;

    TextMeshProUGUI text;
    Image image;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponentInChildren<Image>();
    }

    private float startTime = 0f;

    void Start()
    {
        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        //Debug.Log(Time.time + ", " + startTime);
        if (Time.time - startTime >= durationTime)
        {
            Destroy(this.transform.gameObject);
        }

        if (Time.time - startTime >= durationTime - (durationTime * 0.1f))
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float currentTime = 0;
        float timeToFade = durationTime - (Time.time - startTime);

        Color newTextColor;// = new Color(text.color.r, text.color.g, text.color.b, 0f);
        Color newImageColor;

        //float start = 0;
        while (currentTime < timeToFade)
        {
            currentTime += Time.deltaTime;
            //audioSource.volume = Mathf.Lerp(start, 0, currentTime / timeToFade);
            newTextColor = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(text.color.a, 0f, currentTime / timeToFade));
            text.color = newTextColor;

            if (image != null)
            {
                newImageColor = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, 0f, currentTime / timeToFade));
                image.color = newImageColor;
            }

            yield return null;
        }
        yield break;
    }
}
