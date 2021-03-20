using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototypes.Pathfinding.Scripts
{
    public class Client : MonoBehaviour
    {
        public enum ClientState
        {
            FindingSeat,
            WaitingToOrder,
            WaitingToReciveOrder,
            Eating,
            GoingToPay,
            GettingQuest,
            Leaving,
            Inactive
        }

        private BasicAI mouvement;

        private TextMesh text;

        private TextMesh subText;

        private Seat seat;

        private Entrance exit;

        private GameObject payLocation;

        private ClientState etat;

        private float timeLeftToEat;

        public float timeToEatMin = 5f;
        public float timeToEatMax = 20f;

        private bool hasBeenInteractedWith;

        public bool hasAWaiter;

        public float orderTimer;

        public float timeToMakeOrderMin = 30f;
        public float timeToMakeOrderMax = 100f;


        // Start is called before the first frame update
        void Start()
        {
            mouvement = GetComponent<BasicAI>();
            Transform temp = GetComponent<Transform>().Find("Text");
            text = temp.GetComponent<TextMesh>();
            temp = GetComponent<Transform>().Find("SubText");
            subText = temp.GetComponent<TextMesh>();
            etat = ClientState.FindingSeat;
            hasBeenInteractedWith = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (etat == ClientState.FindingSeat)
            {
                text.text = "Finding Seat";
                if (seat == null)
                {
                    subText.text = "Seat Not Found";
                    seat = mouvement.GoToRandomSeat();
                }
                else
                {
                    subText.text = "Seat Found";
                    if (mouvement.isAtLocation(seat.transform.position))
                    {
                        seat.isOccupied = true;
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        etat = ClientState.WaitingToOrder;
                    }
                }
            }
            else if (etat == ClientState.WaitingToOrder)
            {
                text.text = "Waiting To Order";
                subText.text = "Waiting for Waiter";
                if (hasBeenInteractedWith)
                {
                    etat = ClientState.WaitingToReciveOrder;
                    orderTimer = Random.Range(timeToMakeOrderMin, timeToMakeOrderMax);
                    hasBeenInteractedWith = false;
                }
            }
            else if (etat == ClientState.WaitingToReciveOrder)
            {
                text.text = "Waiting To Recive Order";
                subText.text = "Time Left: " + Mathf.Ceil(orderTimer).ToString() + "s";
                orderTimer -= Time.deltaTime;
                if (hasBeenInteractedWith)
                {
                    etat = ClientState.Eating;
                    timeLeftToEat = Random.Range(timeToEatMin, timeToEatMax);
                    hasBeenInteractedWith = false;
                }
            }
            else if (etat == ClientState.Eating)
            {
                text.text = "Eating";
                subText.text = "Time Left: " + Mathf.Ceil(timeLeftToEat).ToString() + "s";
                timeLeftToEat -= Time.deltaTime;
                if (timeLeftToEat <= 0)
                {
                    etat = ClientState.GoingToPay;
                }
            }
            else if (etat == ClientState.GoingToPay)
            {
                text.text = "Going To Pay";
                if (seat != null)
                {
                    subText.text = "Counter Not Found";
                    seat.isAIGoingForIt = false;
                    seat.isOccupied = false;
                    seat = null;
                    payLocation = mouvement.GoToPay();
                }
                else
                {
                    if (payLocation == null)
                    {
                        subText.text = "Counter Not Found";
                        payLocation = mouvement.GoToPay();
                    }
                    else
                    {
                        subText.text = "Counter Found";
                        if (mouvement.isAtLocation(payLocation.transform.position))
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            etat = ClientState.Leaving;
                        }
                    }
                }
            }
            else if (etat == ClientState.Leaving)
            {
                text.text = "Leaving";
                if (payLocation != null)
                {
                    subText.text = "Exit Not Found";
                    payLocation = null;
                    exit = mouvement.GoToExit();
                }
                else
                {
                    if (exit == null)
                    {
                        subText.text = "Exit Not Found";
                        exit = mouvement.GoToExit();
                    }
                    else
                    {
                        subText.text = "Exit Found";
                        if (mouvement.isAtLocation(exit.transform.position))
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            etat = ClientState.Inactive;
                        }
                    }
                }
            }
            else if (etat == ClientState.Inactive)
            {
                text.text = "Inactive";
                subText.text = "Enable to start over";
                etat = ClientState.FindingSeat;
                transform.gameObject.SetActive(false);
            }
        }

        public void InteractWIthClient()
        {
            hasBeenInteractedWith = true;
        }

        public ClientState getState()
        {
            return etat;
        }

        public Vector3 getPosition()
        {
            return mouvement.getLocation();
        }
    }
}
