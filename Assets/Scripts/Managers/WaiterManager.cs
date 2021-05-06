using System.Collections.Generic;
using Characters;
using Interface;
using UnityEngine;

namespace Managers
{
    public class WaiterManager : MonoBehaviour
    {
        [Header("External Tools")] [SerializeField]
        private ResourcesManager resourcesManager;

        [SerializeField] private GameManager gameManager;


        [Header("Waiter Options")] public GameObject aiWaiterPrefab;
        [SerializeField] private Transform popZone;
        [SerializeField] [Min(0)] private int minAmount;

        [Header("Debug")] [SerializeField] private int targetAmount;
        [SerializeField] private List<Waiter> waiters;

        public int Price { get; set; }
        public int Reputation { get; set; }

        private void Start()
        {
            targetAmount = minAmount;

            waiters = new List<Waiter>();
            SpawnWaiter();
        }

        private void FixedUpdate()
        {
            SpawnWaiter(waiters.Count);
        }

        public void IncreaseWaiterNumber(int n = 1)
        {
            if (resourcesManager.Gold < Price)
            {
                gameManager.NotificationSystem.CreateNotification(
                    $"Not enough money...\n{Price - resourcesManager.Gold} coins missing!", 4f,
                    NotificationType.Warning
                );
                return;
            }

            targetAmount += n;
            resourcesManager.Reputation += Reputation;
            resourcesManager.Gold -= Price;
        }

        private void SpawnWaiter(int n = 0)
        {
            var x = 0;

            for (var i = n; i < targetAmount; i++)
            {
                var waiter = Instantiate(aiWaiterPrefab, popZone).GetComponent<Waiter>();

                waiter.ResourcesManager = resourcesManager;

                waiters.Add(waiter);

                x += 1;
            }

            if (x > 0)
                gameManager.NotificationSystem.CreateNotification(
                    $"Yay!\n{x} new waiter(s) hired!", 4f,
                    NotificationType.Warning
                );
        }
    }
}