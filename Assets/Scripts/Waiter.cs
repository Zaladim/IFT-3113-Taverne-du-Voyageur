using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototypes.Pathfinding.Scripts
{
    public class Waiter : MonoBehaviour
    {
        public enum WaiterState
        {
            Neutral,
            TakingClientOrder,
            GoToCounter,
            GivingOrder
        }

        private BasicAI mouvement;

        private TextMesh text;

        private TextMesh subText;

        private List<Client> clients;

        private Client currentClient;

        private GameObject foodLocation;

        private WaiterState etat;

        public float DistanceFromClientToInterct = 2;

        // Start is called before the first frame update
        void Start()
        {
            mouvement = GetComponent<BasicAI>();
            Transform temp = GetComponent<Transform>().Find("Text");
            text = temp.GetComponent<TextMesh>();
            temp = GetComponent<Transform>().Find("SubText");
            subText = temp.GetComponent<TextMesh>();
            etat = WaiterState.Neutral;
            clients = new List<Client>();
        }

        // Update is called once per frame
        void Update()
        {
            if (etat == WaiterState.Neutral)
            {
                //check if any of its own orders are ready
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].orderTimer <= 0 && clients[i].getState() == Client.ClientState.WaitingToReciveOrder)
                    {
                        currentClient = clients[i];
                        mouvement.GoToSpecificClient(currentClient);
                        etat = WaiterState.GivingOrder;
                    }
                }
                //look for a new client
                if (currentClient == null)
                {
                    text.text = "Finding Client";
                    Client person = mouvement.GoToRandomClient();
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
            }
            else if (etat == WaiterState.TakingClientOrder)
            {
                text.text = "Taking Client's Order";
                subText.text = "";
                if (mouvement.isCloseToLocation(currentClient.getPosition(), DistanceFromClientToInterct))
                {
                    currentClient.InteractWIthClient();
                    etat = WaiterState.GoToCounter;
                }
            }
            else if (etat == WaiterState.GoToCounter)
            {
                text.text = "Going To Counter";
                if (currentClient != null)
                {
                    subText.text = "Counter Not Found";
                    currentClient = null;
                    foodLocation = mouvement.GoToCounter();
                }
                else
                {
                    if (foodLocation == null)
                    {
                        subText.text = "Counter Not Found";
                        foodLocation = mouvement.GoToCounter();
                    }
                    else
                    {
                        subText.text = "Counter Found";
                        if (mouvement.isAtLocation(foodLocation.transform.position))
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            etat = WaiterState.Neutral;
                            foodLocation = null;
                        }
                    }
                }
            }
            else if (etat == WaiterState.GivingOrder)
            {
                text.text = "Giving Client's Order";
                subText.text = "";
                if (mouvement.isCloseToLocation(currentClient.getPosition(), DistanceFromClientToInterct))
                {
                    currentClient.InteractWIthClient();
                    etat = WaiterState.GoToCounter;
                    clients.Remove(currentClient);
                    currentClient = null;
                }
            }
        }
    }
}
