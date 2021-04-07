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
            private uint amount;

            public Resource(Text display, uint amount = 1)
            {
                this.display = display;
                if (display)
                    display.text = amount.ToString();
                this.amount = amount;
            }

            public uint Amount
            {
                get => amount;
                set
                {
                    amount = value;
                    if (display)
                        display.text = amount.ToString();
                }
            }
        }

        private Resource gold;
        [SerializeField] private Text goldDisplay;
        [SerializeField] private uint goldAmount = 25;

        private Resource reputation;
        [SerializeField] private Text reputationDisplay;

        [SerializeField] private int startingSeatNumber = 4;

        private Resource seats;

        private void Awake()
        {
            gold = new Resource(goldDisplay, goldAmount);
            reputation = new Resource(reputationDisplay);
            seats = new Resource(null, (uint) startingSeatNumber);
        }

        public uint Gold
        {
            get => gold.Amount;
            set => gold.Amount = value;
        }

        public int Reputation
        {
            get => (int) reputation.Amount;
            set
            {
                reputation.Amount += (uint) value;
                if (reputation.Amount < 1) reputation.Amount = 1;
            }
        }

        public int Seats
        {
            get => (int) seats.Amount;
            set => seats.Amount += (uint) value;
        }
    }
}