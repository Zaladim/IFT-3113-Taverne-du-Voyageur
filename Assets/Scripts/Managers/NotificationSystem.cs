using Interface;
using UnityEngine;

namespace Managers
{
    public class NotificationSystem : MonoBehaviour
    {
        [SerializeField] private Transform notificationAreaCenter;
        [SerializeField] private Notification notificationWidget;

        public void CreateNotification(string msg, float duration = 3, NotificationType type = NotificationType.None)
        {
            if (!gameObject.activeSelf) return;

            var notification = Instantiate(notificationWidget, notificationAreaCenter);
            notification.SetMessage(msg);
            notification.duration = duration;
            notification.SetType(type);
        }
    }
}