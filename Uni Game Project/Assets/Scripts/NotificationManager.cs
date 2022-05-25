using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationPrefab;
    [SerializeField] int maxNumberOfNotifications = 6;

    [SerializeField] List<GameObject> notifications;
    [SerializeField] Queue<Notification> notificationsQueue;

    private void Awake()
    {
        notificationsQueue = new Queue<Notification>();
    }

    private void FixedUpdate()
    {
        foreach (var obj in notifications)
        {
            if (!obj)
            {
                notifications.Remove(obj);
                break;
            }
        }
        if (notifications.Count < maxNumberOfNotifications)
        {
            if (notificationsQueue != null && notificationsQueue.Count > 0)
            {
                // there is place for a new notification
                Notification dequeuedNotification = notificationsQueue.Dequeue();
                NewNotification(dequeuedNotification.Text, dequeuedNotification.Icon);
            }
        }
    }

    public void NewNotification()
    {
        Debug.Log("New Notification");
        string defaultText = "New Notification";
        NewNotification(defaultText);
    }

    public void NewNotification(string text, Sprite icon = null)
    {
        if (notifications.Count >= maxNumberOfNotifications)
        {
            Notification newNotification = new Notification(text, icon);
            notificationsQueue.Enqueue(newNotification);
            return;
        }
        GameObject notification = Instantiate(notificationPrefab, this.transform);
        notification.transform.localPosition = Vector3.zero;
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();
        notificationText.text = text;
        if (icon != null)
        {
            Image notificationIcon = notification.GetComponentInChildren<Image>();
            notificationIcon.sprite = icon;
        }
        else
        {
            Image notificationIcon = notification.GetComponentInChildren<Image>();
            //destroy(notificationIcon);

            //notificationIcon.enabled = false;
            Destroy(notificationIcon.gameObject);
        }
        notifications.Add(notification);
    }
}

[System.Serializable]
public class Notification
{
    [SerializeField] string text;
    [SerializeField] Sprite icon;

    public Notification()
    {
        text = "";
        icon = null;
    }

    public Notification(string text, Sprite icon)
    {
        this.text = text;
        this.icon = icon;
    }

    public string Text { get => text; set => text = value; }
    public Sprite Icon { get => icon; set => icon = value; }
}