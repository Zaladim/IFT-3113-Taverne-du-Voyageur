using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public enum NotificationType
    {
        Info,
        Warning,
        Danger,
        None
    }

    public class Notification : MonoBehaviour
    {
        [SerializeField] private Text textZone;
        [SerializeField] private Text timeZone;
        [SerializeField] private GameObject infoMark;
        [SerializeField] private GameObject warningMark;
        [SerializeField] private GameObject dangerMark;

        public float duration = 3f;

        public void Update()
        {
            duration -= Time.unscaledDeltaTime;
            timeZone.text = ((int) duration).ToString(CultureInfo.InvariantCulture);

            if (duration <= 0) Destroy(gameObject);
        }

        public void SetType(NotificationType type = NotificationType.None)
        {
            switch (type)
            {
                case NotificationType.Info:
                    infoMark.SetActive(true);
                    break;
                case NotificationType.Warning:
                    warningMark.SetActive(true);
                    break;
                case NotificationType.Danger:
                    dangerMark.SetActive(true);
                    break;
            }
        }

        public void SetMessage(string msg)
        {
            textZone.text = msg;
        }
    }
}