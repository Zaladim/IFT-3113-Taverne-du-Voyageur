using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class RessourcesManager : MonoBehaviour
    {
        [SerializeField] private int gold = 500;
        [SerializeField] private Text display;

        public int Gold
        {
            get => gold;
            set
            {
                gold = value;
                display.text = gold.ToString();
            }
        }

        private void Awake()
        {
            display.text = gold.ToString();
        }
    }
}
