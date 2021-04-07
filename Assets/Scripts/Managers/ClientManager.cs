using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class ClientManager : MonoBehaviour
    {
        [Header("Client Options")] public GameObject aiClientPrefab;
        [SerializeField] private Transform popZone;
        [SerializeField, Min(0)] private int minAmount;
        [SerializeField, Range(1.5f, 10)] private float spawnSpeed = 1.5f;
        [SerializeField] private ResourcesManager rm;

        private int curAmount = 0;
        [SerializeField] private int targetAmount = 0;
        private float timeToSpawn = 0;

        private List<GameObject> clients;

        // Start is called before the first frame update
        private void Start()
        {
            var capacity = rm.Seats;

            targetAmount = minAmount;

            clients = new List<GameObject>(capacity);
            for (var i = 0; i < capacity; i++)
            {
                var client = Instantiate(aiClientPrefab);

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
                var remaining = rm.Seats - targetAmount;
                var n = remaining != 0 ? Random.Range(1, (remaining % 4) + 1) : 0;
                var tmp = Random.Range(0, 100) < (
                    rm.Reputation < 50
                        ? rm.Reputation + 10
                        : rm.Reputation
                )
                    ? n
                    : 0;

                targetAmount += tmp;

                if (curAmount < targetAmount)
                    addNewClient();

                timeToSpawn = spawnSpeed;
                /*Debug.Log(
                    "Seats: " + rm.Seats.ToString() +
                    " Clients: " + curAmount.ToString() +
                    " Target: " + targetAmount.ToString() +
                    " Remaining: " + remaining.ToString()
                );*/
            }
        }

        private void Update()
        {
            for (var i = clients.Count; i < rm.Seats; i++)
            {
                var client = Instantiate(aiClientPrefab);

                client.SetActive(false);
                clients.Add(client);
            }

            curAmount = clients.Count(client => client.activeSelf);
            Debug.Log(
                " Clients: " + curAmount.ToString() +
                " Target: " + targetAmount.ToString()
            );
        }

        private void addNewClient()
        {
            foreach (var client in clients.Where(client => !client.activeSelf))
            {
                client.SetActive(true);
                break;
            }

            curAmount += 1;
        }
    }
}