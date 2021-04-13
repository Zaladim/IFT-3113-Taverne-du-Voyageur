using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;

namespace Pathfinding
{
    public class BasicAI : MonoBehaviour
    {
        public float speedMin = 3f;
        public float speedMax = 7f;
        private Transform centerOfMass;
        private Vector3 currentDestination;
        private int currentNode;
        private List<Node> currentPath;
        private bool hasDestination;
        private Graph pathFinding;
        private float speed;
        private Vector3 target;

        //private Seat currentSeat;

        // Start is called before the first frame update
        private void Start()
        {
            var graph = GameObject.Find("Graph");
            pathFinding = graph.GetComponent<Graph>();
            centerOfMass = GetComponent<Transform>().Find("CenterOfMass");
            Setup();
        }

        // Update is called once per frame
        private void Update()
        {
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

            if (turn) Turn(destination);

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

        public Vector3 GETLocation()
        {
            return centerOfMass.position;
        }

        public bool IsAtLocation(Vector3 position)
        {
            return centerOfMass.position == position;
        }

        public bool IsCloseToLocation(Vector3 position, float minimalDistance)
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
                    while (randomSeat.isAIGoingForIt || randomSeat.isOccupied)
                        randomSeat = allSeats[Random.Range(0, allSeats.Length)];
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

        public GameObject GoToPay(out GameObject lookdirection)
        {
            var allCounters = FindObjectsOfType<Counter>();
            if (allCounters.Length <= 0)
            {
                lookdirection = null;
                return null;
            }

            var randomCounter = allCounters[Random.Range(0, allCounters.Length)];
            lookdirection = randomCounter.lookDirection;
            var paylocation = randomCounter.payLocations[Random.Range(0, randomCounter.payLocations.Count)];
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
            var allAvailableClients = new List<Client>();
            for (var i = 0; i < allClients.Length; i++)
                if (allClients[i].GETState() == ClientState.WaitingToOrder && !allClients[i].HasAWaiter)
                    allAvailableClients.Add(allClients[i]);

            if (allAvailableClients.Count == 0) return null;

            var randomClient = allAvailableClients[Random.Range(0, allAvailableClients.Count)];
            randomClient.HasAWaiter = true;
            target = randomClient.GETPosition();
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return randomClient;
        }

        public GameObject GoToCounter(out GameObject lookdirection)
        {
            var allCounters = FindObjectsOfType<Counter>();
            if (allCounters.Length <= 0)
            {
                lookdirection = null;
                return null;
            }

            var randomCounter = allCounters[Random.Range(0, allCounters.Length)];
            lookdirection = randomCounter.lookDirection;
            var foodlocation = randomCounter.foodLocations[Random.Range(0, randomCounter.foodLocations.Count)];
            target = foodlocation.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return foodlocation;
        }


        public void GoToSpecificClient(Client person)
        {
            target = person.GETPosition();
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
        }

        public GameObject GoToRandomQuestGiver()
        {
            var allQuestGivers = GameObject.FindGameObjectsWithTag("QuestGiver");
            if (allQuestGivers.Length == 0)
            {
                return null;
            }
            
            var randomQuestGiver = allQuestGivers[Random.Range(0, allQuestGivers.Length)];
            target = randomQuestGiver.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
            return randomQuestGiver;
        }

        public void GoToQuestGiver(GameObject vmac)
        {
            target = vmac.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;
        }

        public void StopMoving()
        {
            hasDestination = false;
        }

        public void StartMovingAgain()
        {
            if (currentPath != null) hasDestination = true;
        }

        public void FaceLocation(Vector3 location)
        {
            var dirFromAtoB = (location - transform.position).normalized;
            var dotProd = Vector3.Dot(dirFromAtoB, transform.forward);

            if (dotProd != 1) Turn(location);

            /*Debug.Log(Vector3.Angle(location.forward, transform.position - location.position));
            if (Vector3.Angle(location.forward, transform.position - location.position) > 0)
            {
                Turn(location.position);
            }*/
        }

        public void Setup()
        {
            hasDestination = false;
            speed = Random.Range(speedMin, speedMax);
        }
    }
}