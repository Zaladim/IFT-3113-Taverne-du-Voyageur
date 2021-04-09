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

        [Header("Tools")] [SerializeField] private ClientManager clientManager;
        
        [Header("Gold")] [SerializeField]
        private Text goldDisplay;
        [SerializeField] private int goldAmount = 25;

        [Header("Reputation")] [SerializeField]
        private Text reputationDisplay;
        [SerializeField] private int minReputation = 10;

        [Header("Seats")] [SerializeField]
        private int startingSeatNumber = 4;

        [Header("Beers")] [SerializeField]
        private Text beersDisplay;
        [SerializeField] private int beersAmount = 30;
        
        [Header("Clients")] [SerializeField]
        private Text clientsDisplay;
        
        private Resource beers;
        private Resource clients;
        private Resource gold;
        private Resource seats;
        private Resource reputation;

        private void Awake()
        {
            gold = new Resource(goldDisplay, goldAmount);
            reputation = new Resource(reputationDisplay, minReputation, minReputation);
            seats = new Resource(null, startingSeatNumber);
            beers = new Resource(beersDisplay, beersAmount);
            clients = new Resource(clientsDisplay);
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

        public int Beers
        {
            get => beers.Amount;
            set => beers.Amount = value;
        }

        private void Update()
        {
            clients.Amount = clientManager.ClientsNumber;
        }
    }
}