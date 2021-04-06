using UnityEngine;

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

        // Start is called before the first frame update
        private void Start()
        {
            targetAmount = minAmount;
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
                Debug.Log(
                    "Seats: " + rm.Seats.ToString() + 
                          " Clients: " + curAmount.ToString() + 
                          " Target: " + targetAmount.ToString() + 
                          " Remaining: " + remaining.ToString()
                          );
            }
        }

        private void addNewClient()
        {
            var client = Instantiate(aiClientPrefab, popZone);
            if (client is null) return;

            curAmount += 1;
        }

        public void ClientLeft(int n)
        {
            targetAmount -= n;
            curAmount -= n;
            if (targetAmount < minAmount) targetAmount = minAmount;
        }
    }
}