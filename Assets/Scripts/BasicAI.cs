using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototypes.Pathfinding.Scripts
{
    public class BasicAI : MonoBehaviour
    {
        private Graph pathFinding;

        private Vector3 target;

        private List<Node> currentPath;

        private int currentNode;

        private Vector3 currentDestination;

        private bool hasDestination;

        private Transform centerOfMass;

        public float speedMin = 3f;
        public float speedMax = 7f;

        private float speed;

        //private Seat currentSeat;

        // Start is called before the first frame update
        private void Start()
        {
            var graph = GameObject.Find("Graph");
            pathFinding = graph.GetComponent<Graph>();
            centerOfMass = GetComponent<Transform>().Find("CenterOfMass");
            hasDestination = false;
            speed = Random.Range(speedMin, speedMax);
        }

        // Update is called once per frame
        private void Update()
        {
            //for demo 
            /*if (currentSeat != null)
        {
            if (centerOfMass.position == currentSeat.transform.position)
            {
                currentSeat.isAIGoingForIt = false;
            }
        }

        if (!hasDestination)
        {
            currentSeat = GoToRandomSeat();
        }*/
            //end of demo only code

            //move
            if (hasDestination)
            {
                Move(currentDestination, true);

                if (centerOfMass.position == currentDestination)
                {
                    currentDestination = new Vector3();
                    hasDestination = false;
                }
            }

            //check path
            if (currentPath == null) return;
            if (hasDestination) return;
            currentNode++;
            if (currentNode >= currentPath.Count)
            {
                currentPath = null;
                if (target != new Vector3())
                {
                    currentDestination = target;
                    hasDestination = true;
                }
                else
                {
                    currentDestination = new Vector3();
                    hasDestination = false;
                }

                currentNode = 0;
            }
            else
            {
                currentDestination = currentPath[currentNode].getPosition();
                hasDestination = true;
            }
        }


        private void Move(Vector3 destination, bool turn)
        {
            var currentPosition = centerOfMass.position;

            if (turn)
            {
                Turn(destination);
            }

            transform.position = Vector3.MoveTowards(currentPosition, destination, speed * Time.deltaTime);
        }

        private void Turn(Vector3 destination)
        {
            var direction = (destination - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            var lookRotation = Quaternion.LookRotation(direction);

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime / 2);
        }

        public Vector3 getLocation()
        {
            return centerOfMass.position;
        }

        public bool isAtLocation(Vector3 position)
        {
            return centerOfMass.position == position;
        }

        public bool isCloseToLocation(Vector3 position, float minimalDistance)
        {
            return Vector3.Distance(centerOfMass.position, position) <= minimalDistance;
        }

        public Seat GoToRandomSeat()
        {
            var allSeats = FindObjectsOfType<Seat>();
            var randomSeat = allSeats[Random.Range(0, allSeats.Length)];
            var areAllSeatsOccupied = false;
            if (randomSeat.isAIGoingForIt || randomSeat.isOccupied)
            {
                areAllSeatsOccupied = allSeats.All(t => t.isAIGoingForIt && !randomSeat.isOccupied);

                if (!areAllSeatsOccupied)
                {
                    while (randomSeat.isAIGoingForIt || randomSeat.isOccupied)
                    {
                        randomSeat = allSeats[Random.Range(0, allSeats.Length)];
                    }
                }
            }

            if (areAllSeatsOccupied) return null;
            randomSeat.isAIGoingForIt = true;
            target = randomSeat.transform.position;
            currentPath = pathFinding.A_Star(transform.position, randomSeat.transform.position);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return randomSeat;
        }

        public Entrance GoToExit()
        {
            var allExits = FindObjectsOfType<Entrance>();
            if (allExits.Length <= 0) return null;
            var randomExit = allExits[Random.Range(0, allExits.Length)];
            target = randomExit.transform.position;
            currentPath = pathFinding.A_Star(transform.position, randomExit.transform.position);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return randomExit;
        }

        public GameObject GoToPay()
        {
            var allCounters = FindObjectsOfType<Counter>();
            if (allCounters.Length <= 0) return null;
            var randomCounter = allCounters[Random.Range(0, allCounters.Length)];
            GameObject paylocation = randomCounter.payLocations[Random.Range(0, randomCounter.payLocations.Count)];
            target = paylocation.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return paylocation;
        }

        public Client GoToRandomClient()
        {
            var allClients = FindObjectsOfType<Client>();
            List<Client> allAvailableClients = new List<Client>();
            for (int i = 0; i < allClients.Length; i++)
            {
                if (allClients[i].getState() == Client.ClientState.WaitingToOrder && !allClients[i].hasAWaiter)
                {
                    allAvailableClients.Add(allClients[i]);
                }
            }
            if (allAvailableClients.Count == 0)
            {
                return null;
            }
            else
            {
                var randomClient = allAvailableClients[Random.Range(0, allAvailableClients.Count)];
                randomClient.hasAWaiter = true;
                target = randomClient.getPosition();
                currentPath = pathFinding.A_Star(transform.position, target);
                currentNode = 0;
                currentDestination = currentPath[currentNode].getPosition();
                hasDestination = true;
                return randomClient;
            }
        }

        public GameObject GoToCounter()
        {
            var allCounters = FindObjectsOfType<Counter>();
            if (allCounters.Length <= 0) return null;
            var randomCounter = allCounters[Random.Range(0, allCounters.Length)];
            GameObject foodlocation = randomCounter.foodLocations[Random.Range(0, randomCounter.foodLocations.Count)];
            target = foodlocation.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return foodlocation;
        }


        public void GoToSpecificClient(Client person)
        {
            target = person.getPosition();
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
        }
    }
}