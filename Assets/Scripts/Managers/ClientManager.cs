using System.Collections.Generic;
using System.Linq;
using Characters;
using Interface;
using UnityEngine;

namespace Managers
{
    public class ClientManager : MonoBehaviour
    {
        [Header("External Tools")] [SerializeField]
        private ResourcesManager resourcesManager;

        [SerializeField] private GameManager gameManager;

        [Header("Client Options")] public GameObject aiClientPrefab;
        [SerializeField] private Transform popZone;
        [SerializeField] [Min(0)] private int minAmount;
        [SerializeField] [Range(1.5f, 10)] private float spawnSpeed = 1.5f;
        [SerializeField] [Range(1.5f, 10)] private float lurkSpeed = 1.5f;

        [Header("Debug")] [SerializeField] private int targetAmount;
        [SerializeField] private int curAmount;
        [SerializeField] private int deltaAmount;
        [SerializeField] private float spawnTimer;
        [SerializeField] private float lurkTimer;
        [SerializeField] private List<Client> clients;
        [SerializeField] private bool isFull = false;

        public int ClientsNumber => curAmount;

        private void Start()
        {
            targetAmount = minAmount;

            clients = new List<Client>();
            SpawnClients();
        }

        private void Update()
        {
            SpawnClients(clients.Count);

            var tmp = clients.Count(client => client.gameObject.activeSelf);
            curAmount = tmp;
            deltaAmount = resourcesManager.Seats - targetAmount;
            isFull = deltaAmount <= 0;
        }

        private void FixedUpdate()
        {
            var elapsedTime = Time.fixedDeltaTime;
            var reputation = resourcesManager.Reputation;

            if (spawnTimer > 0)
            {
                spawnTimer -= elapsedTime;
            }
            else
            {
                var n = deltaAmount >= 0 ? Random.Range(1, deltaAmount % 4 + 1) : 0;
                var tmp = Random.Range(0, 100 > reputation ? 100 : reputation) < reputation ? n : 0;

                if (targetAmount <= reputation)
                    targetAmount += tmp;
                else
                    targetAmount = reputation;

                if (curAmount < targetAmount)
                    AddNewClient(targetAmount - curAmount);

                spawnTimer = spawnSpeed;
            }

            if (lurkTimer > 0)
            {
                lurkTimer -= elapsedTime;
            }
            else if (isFull)
            {
                if (Random.Range(0, 100 > reputation ? 100 : reputation) < reputation)
                    AddNewClient(Random.Range(0, 2));
                lurkTimer = lurkSpeed;
            }
        }

        private void AddNewClient(int n = 1)
        {
            for (var i = 0; i < n; i++)
                foreach (var client in clients.Where(client => !client.gameObject.activeSelf))
                {
                    client.gameObject.SetActive(true);
                    curAmount += 1;
                    break;
                }
        }

        private void SpawnClients(int n = 0)
        {
            for (var i = n; i < targetAmount; i++)
            {
                var client = Instantiate(aiClientPrefab, popZone).GetComponent<Client>();

                client.gameObject.SetActive(false);
                client.ResourcesManager = resourcesManager;
                client.GameManager = gameManager;
                clients.Add(client);
            }
        }
    }
}