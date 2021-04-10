using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        [Header("Tools")] [SerializeField] private ClientManager clientManager;

        [Header("Gold")] [SerializeField] private Text goldDisplay;
        [SerializeField] private int startingGoldAmount = 25;

        [Header("Reputation")] [SerializeField]
        private Text reputationDisplay;

        [SerializeField] private int StartingReputation = 10;
        [SerializeField] private int minReputation = 10;

        [Header("Seats")] [SerializeField] private int startingSeatNumber = 4;

        [Header("Beers")] [SerializeField] private Text beersDisplay;
        [SerializeField] private int startingBeersAmount = 30;
        [SerializeField] private int beerPrice = 5;

        [Header("Clients")] [SerializeField] private Text clientsDisplay;

        [Header("Debug")] [SerializeField] [Tooltip("! Does nothing if changed in editor !")]
        private int clientsQuantity;

        [SerializeField] [Tooltip("! Does nothing if changed in editor !")]
        private int goldQuantity;

        [SerializeField] [Tooltip("! Does nothing if changed in editor !")]
        private int reputationQuantity;

        [SerializeField] [Tooltip("! Does nothing if changed in editor !")]
        private int beersQuantity;

        [SerializeField] [Tooltip("! Does nothing if changed in editor !")]
        private int seatsQuantity;

        private Resource beers;
        private Resource clients;
        private Resource gold;
        private Resource reputation;
        private Resource seats;

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

        private void Awake()
        {
            gold = new Resource(goldDisplay, startingGoldAmount);
            reputation = new Resource(reputationDisplay, StartingReputation, minReputation);
            seats = new Resource(null, startingSeatNumber);
            beers = new Resource(beersDisplay, startingBeersAmount);
            clients = new Resource(clientsDisplay);
        }

        private void Update()
        {
            clients.Amount = clientManager.ClientsNumber;

            // debug update
            clientsQuantity = clients.Amount;
            goldQuantity = Gold;
            reputationQuantity = Reputation;
            beersQuantity = Beers;
            seatsQuantity = Seats;
        }

        public void BuyBeers(int n)
        {
            var tmp = n * beerPrice;

            if (tmp <= Gold && n != -1)
            {
                Beers += n;
                Gold -= tmp;
            }
            else
            {
                var max = (int) Math.Floor((decimal) (Gold / (float) beerPrice));
                Beers += max;
                Gold -= max * beerPrice;
            }
        }

        private class Resource
        {
            private readonly Text display;
            private readonly int min;
            private int amount;

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
    }
}