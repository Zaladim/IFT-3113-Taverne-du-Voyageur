using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;
using System.Threading;

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
        [SerializeField] private Vector3 target;

        private Thread thread;
        //temporary variables for the thread
        [SerializeField] private Node currentPosition;
        [SerializeField] private Node destinationPosition;

        private bool graphUpdateScheduled;

        //private Seat currentSeat;

        // Start is called before the first frame update
        private void Start()
        {
            var graph = GameObject.Find("Graph");
            pathFinding = graph.GetComponent<Graph>().CopyGraph();
            centerOfMass = GetComponent<Transform>().Find("CenterOfMass");
            Setup();
        }

        // Update is called once per frame
        private void Update()
        {
            if (graphUpdateScheduled && (thread == null || thread != null && !thread.IsAlive))
            {
                graphUpdateScheduled = false;
                pathFinding.UpdateGraph();
                //Debug.Log(gameObject.name + " has updated its graph");
            }

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

        public void ScheduledGraphUpdate()
        {
            graphUpdateScheduled = true;
        }

        public bool IsThreadRunning()
        {
            if (thread == null)
            {
                return false;
            }

            return thread.ThreadState == ThreadState.Running;
        }

        private void SetNewPath(Vector3 destination)
        {
            if (currentPath == null || pathFinding.getColsestNodeToPoint(destination) != currentPath[currentPath.Count - 1])// si le chemin courant ne donnem pas déjà vers la même destination
            {
                if (IsThreadRunning())
                {
                    pathFinding.Kill();
                    while (IsThreadRunning())
                    {
                        //just waiting for thread to stop
                    }
                    pathFinding.Resurect();
                    thread = null;
                }
                if (graphUpdateScheduled)
                {
                    graphUpdateScheduled = false;
                    pathFinding.UpdateGraph();
                }
                currentPosition = pathFinding.getColsestNodeToPoint(transform.position);
                destinationPosition = pathFinding.getColsestNodeToPoint(destination);
                target = destination;
                this.thread = new Thread(() => this.getPath());
                this.thread.Start();
            }
        }

        private void getPath()
        {
            currentPath = pathFinding.A_Star(currentPosition, destinationPosition);
            if (currentPath != null)
            {
                currentNode = 0;
                currentDestination = currentPath[currentNode].getPosition();
                hasDestination = true;
            }
        }

        public Seat GoToRandomSeat()
        {
            var allSeats = FindObjectsOfType<Seat>();
            var randomSeat = allSeats[Random.Range(0, allSeats.Length)];
            var areAllSeatsOccupied = false;
            if (randomSeat.isAIGoingForIt || randomSeat.isOccupied)
            {
                areAllSeatsOccupied = allSeats.All(t => t.isAIGoingForIt);

                if (!areAllSeatsOccupied)
                    while (randomSeat.isAIGoingForIt || randomSeat.isOccupied)
                        randomSeat = allSeats[Random.Range(0, allSeats.Length)];
            }

            if (areAllSeatsOccupied) return null;
            randomSeat.isAIGoingForIt = true;
            SetNewPath(randomSeat.transform.position);
            //getPath(transform.position, randomSeat.transform.position);
            /*target = randomSeat.transform.position;
            currentPath = pathFinding.A_Star(transform.position, randomSeat.transform.position);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
            return randomSeat;
        }

        public Entrance GoToExit()
        {
            var allExits = FindObjectsOfType<Entrance>();
            if (allExits.Length <= 0) return null;
            var randomExit = allExits[Random.Range(0, allExits.Length)];
            SetNewPath(randomExit.transform.position);
            /*target = randomExit.transform.position;
            currentPath = pathFinding.A_Star(transform.position, randomExit.transform.position);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
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
            SetNewPath(paylocation.transform.position);
            /*target = paylocation.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
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
            SetNewPath(randomClient.GETPosition());
            /*target = randomClient.GETPosition();
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
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
            SetNewPath(foodlocation.transform.position);
            /*target = foodlocation.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
            return foodlocation;
        }


        public void GoToSpecificClient(Client person)
        {
            SetNewPath(person.GETPosition());
            /*target = person.GETPosition();
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
        }

        public GameObject GoToRandomQuestGiver()
        {
            var allQuestGivers = GameObject.FindGameObjectsWithTag("QuestGiver");
            if (allQuestGivers.Length == 0)
            {
                return null;
            }

            var randomQuestGiver = allQuestGivers[Random.Range(0, allQuestGivers.Length)];

            SetNewPath(randomQuestGiver.transform.position);
            /*target = randomQuestGiver.transform.position;
            currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
            return randomQuestGiver;
        }

        public void GoToQuestGiver(GameObject vmac)
        {
            SetNewPath(vmac.transform.position);
            /*target = vmac.transform.position;
            //currentPath = pathFinding.A_Star(transform.position, target);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
        }

        public Node GoToRandomLookAroundNode()
        {
            var allNodes = pathFinding.getNodes();
            if (allNodes.Count() == 0)
            {
                return null;
            }
            var randomnode = allNodes[Random.Range(0, allNodes.Count())];
            while (randomnode.isntALookAroundNode)
            {
                randomnode = allNodes[Random.Range(0, allNodes.Count())];
            }

            SetNewPath(randomnode.getPosition());
            //getPath(transform.position, randomSeat.transform.position);
            /*target = randomSeat.transform.position;
            currentPath = pathFinding.A_Star(transform.position, randomSeat.transform.position);
            currentNode = 0;
            currentDestination = currentPath[currentNode].getPosition();
            hasDestination = true;*/
            return randomnode;
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

        public bool CheckIfPathNotFound()
        {
            if (thread != null && !thread.IsAlive)
            {
                return currentPath == null;
            }
            else
            {
                return false;
            }
        }
    }
}