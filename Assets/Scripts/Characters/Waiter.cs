using System.Collections.Generic;
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
        [Header("Options")]
        [SerializeField] private float distanceFromClientToInteract = 2;
        [SerializeField] private float timeToStayIdleMin = 1f;
        [SerializeField] private float timeToStayIdleMax = 10f;
        
        [Header("Debug")]
        [SerializeField] private float idleTimer;
        [SerializeField] private List<Client> clients;
        [SerializeField] private Client currentClient;
        [SerializeField] private WaiterState etat;
        [SerializeField] private GameObject foodLocation;
        [SerializeField] private GameObject lookDirection;
        [SerializeField] private BasicAI mouvement;
        [SerializeField] private TextMesh subText;
        [SerializeField] private TextMesh text;

        // Start is called before the first frame update
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

                    //check if any of its own orders are ready
                    for (var i = 0; i < clients.Count; i++)
                        if (clients[i].TimeLeft <= 10 &&
                            clients[i].GETState() == ClientState.WaitingToReciveOrder)
                        {
                            currentClient = clients[i];
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
                    text.text = "Going To Counter";
                    if (currentClient != null)
                    {
                        subText.text = "Counter Not Found";
                        currentClient = null;
                        foodLocation = mouvement.GoToCounter(out lookDirection);
                    }
                    else
                    {
                        if (foodLocation == null)
                        {
                            subText.text = "Counter Not Found";
                            foodLocation = mouvement.GoToCounter(out lookDirection);
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