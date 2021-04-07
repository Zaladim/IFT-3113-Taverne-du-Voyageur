using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class WaiterManager : MonoBehaviour
    {
        [Header("External Tools")] [SerializeField]
        private ResourcesManager rm;

        [Header("Waiter Options")] public GameObject aiWaiterPrefab;
        [SerializeField] private Transform popZone;
        [SerializeField, Min(0)] private int minAmount;

        [Header("Debug")] [SerializeField] private int targetAmount = 0;
        [SerializeField] private List<GameObject> waiters;

        public int Price { get; set; }
        public int Reputation { get; set; }

        private void Start()
        {
            targetAmount = minAmount;

            waiters = new List<GameObject>(targetAmount);
            for (var i = 0; i < targetAmount; i++)
            {
                var waiter = Instantiate(aiWaiterPrefab, popZone);

                waiter.SetActive(true);
                waiters.Add(waiter);
            }
        }

        private void Update()
        {
            for (var i = waiters.Count; i < targetAmount; i++)
            {
                var waiter = Instantiate(aiWaiterPrefab, popZone);

                waiter.SetActive(true);
                waiters.Add(waiter);
            }
        }

        public void IncreaseWaiterNumber(int n = 1)
        {
            if (rm.Gold < Price) return;
            
            targetAmount += n;
            rm.Reputation += Reputation;
            rm.Gold -= Price;
        }
    }
}