using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class ClientManager : MonoBehaviour
    {
        [Header("External Tools")] [SerializeField]
        private ResourcesManager rm;

        [Header("Client Options")] public GameObject aiClientPrefab;
        [SerializeField] private Transform popZone;
        [SerializeField, Min(0)] private int minAmount;
        [SerializeField, Range(1.5f, 10)] private float spawnSpeed = 1.5f;

        [Header("Debug")] [SerializeField] private int targetAmount = 0;
        [SerializeField] private int curAmount = 0;
        [SerializeField] private int deltaAmount = 0;
        [SerializeField] private float timeToSpawn = 0;
        [SerializeField] private List<GameObject> clients;

        private void Start()
        {
            var capacity = rm.Seats;

            targetAmount = minAmount;

            clients = new List<GameObject>(capacity);
            for (var i = 0; i < capacity; i++)
            {
                var client = Instantiate(aiClientPrefab, popZone);

                client.SetActive(false);
                clients.Add(client);
            }
        }

        private void FixedUpdate()
        {
            if (timeToSpawn > 0)
            {
                timeToSpawn -= Time.fixedDeltaTime;
            }
            else
            {
                var reputation = rm.Reputation;

                var n = deltaAmount != 0 ? Random.Range(1, (deltaAmount % 4) + 1) : 0;
                var tmp = Random.Range(0, 100 > reputation ? 100 : reputation) < reputation ? n : 0;

                if (targetAmount <= reputation)
                {
                    targetAmount += tmp;
                }
                else
                {
                    targetAmount = reputation;
                }

                if (curAmount < targetAmount)
                    AddNewClient(targetAmount - curAmount);

                timeToSpawn = spawnSpeed;
            }
        }

        private void Update()
        {
            for (var i = clients.Count; i < rm.Seats; i++)
            {
                var client = Instantiate(aiClientPrefab, popZone);

                client.SetActive(false);
                clients.Add(client);
            }

            var tmp = clients.Count(client => client.activeSelf);
            curAmount = tmp;
            deltaAmount = rm.Seats - targetAmount;
        }

        private void AddNewClient(int n = 1)
        {
            for (var i = 0; i < n; i++)
            {
                foreach (var client in clients.Where(client => !client.activeSelf))
                {
                    client.SetActive(true);
                    curAmount += 1;
                    break;
                }
            }
        }
    }
}