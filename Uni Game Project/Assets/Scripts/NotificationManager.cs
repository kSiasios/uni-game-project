using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationPrefab;

    public void NewNotification()
    {
        Debug.Log("New Notification");
        string defaultText = "New Notification";
        NewNotification(defaultText);
    }

    public void NewNotification(string text, Sprite icon = null)
    {
        GameObject notification = Instantiate(notificationPrefab, this.transform);
        notification.transform.localPosition = Vector3.zero;
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();
        notificationText.text = text;
        if (icon != null)
        {
            Image notificationIcon = notification.GetComponentInChildren<Image>();
            notificationIcon.sprite = icon;
        } else
        {
            Image notificationIcon = notification.GetComponentInChildren<Image>();
            //destroy(notificationIcon);

            //notificationIcon.enabled = false;
            Destroy(notificationIcon.gameObject);
        }
    }
}
