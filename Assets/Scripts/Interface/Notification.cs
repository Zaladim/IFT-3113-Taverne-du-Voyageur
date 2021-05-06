using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private Text textZone;
        [SerializeField] private Text timeZone;
        public float duration = 3f;

        public void Update()
        {
            duration -= Time.unscaledDeltaTime;
            timeZone.text = ((int) duration).ToString(CultureInfo.InvariantCulture);

            if (duration <= 0) Destroy(gameObject);
        }

        public void SetMessage(string msg)
        {
            textZone.text = msg;
        }
    }
}