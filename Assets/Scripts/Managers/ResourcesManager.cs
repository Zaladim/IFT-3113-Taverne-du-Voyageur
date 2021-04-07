using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        private class Resource
        {
            private readonly Text display;
            private int amount;
            private readonly int min;

            public Resource(Text display, int amount = 1, int min = 0)
            {
                this.display = display;
                if (display)
                    display.text = amount.ToString();
                this.amount = amount;
                this.min = min;
            }

            public int Amount
            {
                get => amount;
                set
                {
                    amount = value >= min ? value : min;
                    if (display)
                        display.text = amount.ToString();
                }
            }
        }

        [Header("Gold")] private Resource gold;
        [SerializeField] private Text goldDisplay;
        [SerializeField] private int goldAmount = 25;

        [Header("Reputation")] private Resource reputation;
        [SerializeField] private Text reputationDisplay;
        [SerializeField] private int minReputation = 10;

        [Header("Seats")] private Resource seats;
        [SerializeField] private int startingSeatNumber = 4;

        private void Awake()
        {
            gold = new Resource(goldDisplay, goldAmount);
            reputation = new Resource(reputationDisplay, minReputation, minReputation);
            seats = new Resource(null, startingSeatNumber);
        }

        public int Gold
        {
            get => gold.Amount;
            set => gold.Amount = value;
        }

        public int Reputation
        {
            get => reputation.Amount;
            set => reputation.Amount = value;
        }

        public int Seats
        {
            get => seats.Amount;
            set => seats.Amount = value;
        }
    }
}