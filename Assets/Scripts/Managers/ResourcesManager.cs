using System;
using Interface;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ResourcesManager : MonoBehaviour
    {
        [Header("Tools")] [SerializeField] private ClientManager clientManager;
        [SerializeField] private GameManager gameManager;

        [Header("Gold")] [SerializeField] private Text goldDisplay;
        [SerializeField] private int startingGoldAmount = 25;

        [Header("Reputation")] [SerializeField]
        private Text reputationDisplay;

        [SerializeField] private int startingReputation = 10;
        [SerializeField] private int minReputation = 10;

        [Header("Seats")] [SerializeField] private int startingSeatNumber = 4;

        [Header("Beers")] [SerializeField] private Text beersDisplay;
        [SerializeField] private int startingBeersAmount = 30;
        [SerializeField] private int beerPrice = 5;

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

        [SerializeField] private bool reputationWarned;
        [SerializeField] private bool goldWarned;
        [SerializeField] private bool beerWarned;

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
            reputation = new Resource(reputationDisplay, startingReputation, minReputation);
            seats = new Resource(null, startingSeatNumber);
            beers = new Resource(beersDisplay, startingBeersAmount);
            clients = new Resource(null);

            reputationWarned = false;
            goldWarned = false;
            beerWarned = false;
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

        private void FixedUpdate()
        {
            if (reputation.Amount < 10 && !reputationWarned)
            {
                gameManager.NotificationSystem.CreateNotification("Your reputation is getting low...!", 2f,
                    NotificationType.Warning);
                reputationWarned = true;
            }
            else if (reputation.Amount >= 10)
            {
                reputationWarned = false;
            }
            else if (reputation.Amount == 0)
            {
                gameManager.NotificationSystem.CreateNotification(
                    "Ouch...!\nYour establishment does not seem to be VERY popular...\nMaybe you'll do better next time!",
                    10f, NotificationType.Danger);
                gameManager.GameForcePause = true;
            }

            if (gold.Amount < 7 && !goldWarned)
            {
                gameManager.NotificationSystem.CreateNotification("Your coins reserve is getting low...!", 2f,
                    NotificationType.Warning);
                goldWarned = true;
            }
            else if (gold.Amount >= 7)
            {
                goldWarned = false;
            }

            if (beers.Amount < 3 && !beerWarned)
            {
                gameManager.NotificationSystem.CreateNotification("Your coins reserve is getting low...!", 2f,
                    NotificationType.Warning);
                beerWarned = true;
            }
            else if (beers.Amount >= 3)
            {
                beerWarned = false;
            }
        }

        public void BuyBeers(int n)
        {
            var tmp = n * beerPrice;

            if (tmp > Gold)
            {
                gameManager.NotificationSystem.CreateNotification(
                    $"Not enough money... \n {tmp - Gold} coins missing!", 4f, NotificationType.Warning
                );
                return;
            }

            if (tmp <= Gold && n != -1)
            {
                Beers += n;
                Gold -= tmp;
                
                gameManager.NotificationSystem.CreateNotification(
                    $"Huzzah!!\n{n} beers added!", 4f, NotificationType.Info
                );
            }
            else
            {
                var max = (int) Math.Floor((decimal) (Gold / (float) beerPrice));
                Beers += max;
                Gold -= max * beerPrice;
                
                gameManager.NotificationSystem.CreateNotification(
                    $"Huzzah!!\n{max} beers added!", 4f, NotificationType.Info
                );
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