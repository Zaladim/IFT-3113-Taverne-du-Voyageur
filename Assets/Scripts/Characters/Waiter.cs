using System.Collections.Generic;
using System.Linq;
using Managers;
using Pathfinding;
using UnityEngine;

namespace Characters
{
    public enum WaiterState
    {
        Neutral,
        TakingClientOrder,
        GoToCounter,
        GivingOrder,
        Idle //wait a little bit before leaving client to go back to counter
    }

    public class Waiter : MonoBehaviour
    {
        [Header("Props")] [SerializeField] private Animator anim;

        [Header("Options")] [SerializeField] private float distanceFromClientToInteract = 2;
        [SerializeField] private float timeToStayIdleMin = 1f;
        [SerializeField] private float timeToStayIdleMax = 10f;
        [SerializeField] private ResourcesManager resourcesManager;
        [SerializeField] private float timeToWaitBeforeLookingForDestinationAgain = 60f;

        [Header("Debug")] [SerializeField] private float idleTimer;
        [SerializeField] private List<Client> clients;
        [SerializeField] private Client currentClient;
        [SerializeField] private WaiterState etat;
        [SerializeField] private GameObject foodLocation;
        [SerializeField] private GameObject lookDirection;
        [SerializeField] private BasicAI mouvement;
        [SerializeField] private TextMesh subText;
        [SerializeField] private TextMesh text;
        [SerializeField] private float newDestinationTimer;

        public ResourcesManager ResourcesManager
        {
            get => resourcesManager;
            set => resourcesManager = value;
        }

        private void Start()
        {
            mouvement = GetComponent<BasicAI>();
            var temp = GetComponent<Transform>().Find("Text");
            text = temp.GetComponent<TextMesh>();
            temp = GetComponent<Transform>().Find("SubText");
            subText = temp.GetComponent<TextMesh>();
            etat = WaiterState.Neutral;
            idleTimer = 0;
            clients = new List<Client>();
        }

        // Update is called once per frame
        private void Update()
        {
            switch (etat)
            {
                case WaiterState.Neutral:
                    if (lookDirection != null) mouvement.FaceLocation(lookDirection.transform.position);
                    anim.SetBool("moving", false);

                    //check if any of its own orders are ready
                    foreach (var client in
                        clients.Where(
                            client => client.TimeLeft <= 10 &&
                                      client.GETState() ==
                                      ClientState.WaitingToReciveOrder)
                    )
                    {
                        if (resourcesManager.Beers <= 0) continue;
                        currentClient = client;
                        resourcesManager.Beers -= 1;
                        mouvement.GoToSpecificClient(currentClient);
                        etat = WaiterState.GivingOrder;
                    }

                    //look for a new client
                    if (currentClient == null)
                    {
                        text.text = "Finding Client";
                        var person = mouvement.GoToRandomClient();
                        if (person == null)
                        {
                            subText.text = "Client Not Found";
                        }
                        else
                        {
                            subText.text = "Client Found";
                            currentClient = person;
                            clients.Add(person);
                            etat = WaiterState.TakingClientOrder;
                        }
                    }

                    break;
                case WaiterState.TakingClientOrder:
                    anim.SetBool("moving", true);
                    text.text = "Taking Client's Order";
                    subText.text = "";
                    if (mouvement.IsCloseToLocation(currentClient.GETPosition(), distanceFromClientToInteract))
                    {
                        currentClient.InteractWIthClient();
                        etat = WaiterState.Idle;
                        idleTimer = Random.Range(timeToStayIdleMin, timeToStayIdleMax);
                    }

                    break;
                case WaiterState.GoToCounter:
                    anim.SetBool("moving", true);
                    text.text = "Going To Counter";
                    if (currentClient != null)
                    {
                        subText.text = "Counter Not Found";
                        currentClient = null;
                        foodLocation = mouvement.GoToCounter(out lookDirection);
                        newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                    }
                    else
                    {
                        if (foodLocation == null)
                        {
                            subText.text = "Counter Not Found";
                            if (newDestinationTimer > 0)
                            {
                                newDestinationTimer -= Time.deltaTime;
                            }
                            else
                            {
                                newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                                foodLocation = mouvement.GoToCounter(out lookDirection);
                            }
                        }
                        else
                        {
                            subText.text = "Counter Found";
                            if (mouvement.IsAtLocation(foodLocation.transform.position))
                            {
                                if (lookDirection == null) transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                                etat = WaiterState.Neutral;
                                foodLocation = null;
                            }
                        }
                    }

                    break;
                case WaiterState.GivingOrder:
                    anim.SetBool("moving", false);
                    text.text = "Giving Client's Order";
                    subText.text = "";
                    if (mouvement.IsCloseToLocation(currentClient.GETPosition(), distanceFromClientToInteract))
                    {
                        currentClient.InteractWIthClient();
                        etat = WaiterState.Idle;
                        idleTimer = Random.Range(timeToStayIdleMin, timeToStayIdleMax);
                        clients.Remove(currentClient);
                    }

                    break;
                case WaiterState.Idle:
                    anim.SetBool("moving", false);
                    mouvement.StopMoving();
                    mouvement.FaceLocation(currentClient.gameObject.transform.position);
                    subText.text = "Time Left: " + Mathf.Ceil(idleTimer) + "s";
                    idleTimer -= Time.deltaTime;

                    if (idleTimer <= 0) etat = WaiterState.GoToCounter;

                    break;
            }
        }
    }
}