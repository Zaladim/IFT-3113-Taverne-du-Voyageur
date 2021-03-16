using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototypes.Environnement_Vivant.Scripts
{
    public class ClientManagerBeta : MonoBehaviour
    {
        [Header("Client Options")] public GameObject aiClientPrefab;
        [SerializeField] private Transform popZone;
        [SerializeField, Min(0)] private int minAmount = 0;
        [SerializeField, Min(0)] private int roomMaximumCapacity = 64;
        private int quartAmount;
        private int halfAmount;
        private int minusQuartAmount;
        private int curAmount;
        private int targetAmount;
        private readonly Queue<GameObject> clients = new Queue<GameObject>();
        [SerializeField, Range(1.5f, 10)] private float spawnSpeed = 1.5f;
        private float timeToSpawn;

        [Header("UI Options")] [SerializeField]
        private Text minTextValue;
        [SerializeField] private Text currentTextValue;
        [SerializeField] private Text maxTextValue;
        [SerializeField] private Text targetTextValue;

        // Start is called before the first frame update
        private void Start()
        {
            quartAmount = (int) (roomMaximumCapacity * 0.25f);
            halfAmount = (int) (roomMaximumCapacity * 0.5f);
            minusQuartAmount = (int) (roomMaximumCapacity * 0.75f);

            minTextValue.text = minAmount.ToString();
            maxTextValue.text = roomMaximumCapacity.ToString();

            timeToSpawn = spawnSpeed;
            targetAmount = minAmount;
        }

        private void Update()
        {
            if (timeToSpawn > 0)
            {
                timeToSpawn -= Time.deltaTime;
            }
            else
            {
                if (curAmount < targetAmount)
                    addNewClient();
                else if (curAmount > targetAmount)
                    removeClient();
            }
        }

        public void roomEmpty()
        {
            targetAmount = minAmount;
            targetTextValue.text = $"{targetAmount}";
        }

        public void roomFull()
        {
            targetAmount = roomMaximumCapacity;
            targetTextValue.text = $"{targetAmount}";
        }

        public void roomQuart()
        {
            targetAmount = quartAmount;
            targetTextValue.text = $"{targetAmount}";
        }

        public void roomHalf()
        {
            targetAmount = halfAmount;
            targetTextValue.text = $"{targetAmount}";
        }

        public void roomFullMinusQuart()
        {
            targetAmount = minusQuartAmount;
            targetTextValue.text = $"{targetAmount}";
        }

        public void clientSpawn()
        {
            targetAmount += 1;
            targetTextValue.text = $"{targetAmount}";
        }

        public void clientDeSpawn()
        {
            if (targetAmount <= minAmount) return;
            targetAmount -= 1;
            targetTextValue.text = $"{targetAmount}";
        }


        private void addNewClient()
        {
            var nc = Instantiate(aiClientPrefab, popZone);
            if (nc is null) return;
            curAmount += 1;
            clients.Enqueue(nc);
            currentTextValue.text = $"{curAmount}";
            timeToSpawn = spawnSpeed;
        }

        private void removeClient()
        {
            if (curAmount == minAmount) return;

            var dc = clients.Dequeue();
            if (dc is null) return;
            curAmount -= 1;
            Destroy(dc);
            currentTextValue.text = $"{curAmount}";
            timeToSpawn = spawnSpeed;
        }
    }
}