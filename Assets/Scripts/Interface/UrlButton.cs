using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Interface
{
    public class UrlButton : MonoBehaviour
    {
        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
    }
}