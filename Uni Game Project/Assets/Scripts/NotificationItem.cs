using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationItem : MonoBehaviour
{
    [SerializeField] float durationTime;

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
    }
}
