using UnityEngine;

namespace Interface
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private GameObject element;
        [SerializeField] private bool defaultVisibility = false;

        private void Start()
        {
            element.SetActive(defaultVisibility);
        }

        public void ToggleElement()
        {
            defaultVisibility = !element.activeSelf;
            element.SetActive(defaultVisibility);
        }
    }
}