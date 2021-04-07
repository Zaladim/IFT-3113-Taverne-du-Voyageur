using Managers;
using UnityEngine;

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

    private GameObject lookDirection;

    private ClientState etat;

    private float timeLeftToEat;

    public float timeToEatMin = 5f;
    public float timeToEatMax = 20f;

    private bool hasBeenInteractedWith;

    public bool hasAWaiter;

    public float orderTimer;

    public float timeToMakeOrderMin = 30f;
    public float timeToMakeOrderMax = 100f;

    public int orderPriceMin = 10;
    public int orderPriceMax = 40;

    private bool isHappy = true;
    private uint price;


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
        price = (uint) Random.Range(orderPriceMin, orderPriceMax);
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
                lookDirection = seat.lookDirection;
            }
            else
            {
                subText.text = "Seat Found";
                if (mouvement.isAtLocation(seat.transform.position))
                {
                    seat.isOccupied = true;
                    if (lookDirection == null)
                    {
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }

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

            if (lookDirection != null)
            {
                mouvement.FaceLocation(lookDirection.transform.position);
            }
        }
        else if (etat == ClientState.WaitingToReciveOrder)
        {
            text.text = "Waiting To Recive Order";
            subText.text = Mathf.Ceil(orderTimer).ToString() + "s";
            orderTimer -= Time.deltaTime;
            isHappy = orderTimer >= -10;

            if (hasBeenInteractedWith)
            {
                etat = ClientState.Eating;
                timeLeftToEat = Random.Range(timeToEatMin, timeToEatMax);
                hasBeenInteractedWith = false;
            }

            if (lookDirection != null)
            {
                mouvement.FaceLocation(lookDirection.transform.position);
            }
        }
        else if (etat == ClientState.Eating)
        {
            text.text = "Eating";
            subText.text = Mathf.Ceil(timeLeftToEat).ToString() + "s";
            timeLeftToEat -= Time.deltaTime;
            if (timeLeftToEat <= 0)
            {
                etat = ClientState.GoingToPay;
            }

            if (lookDirection != null)
            {
                mouvement.FaceLocation(lookDirection.transform.position);
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
                payLocation = mouvement.GoToPay(out lookDirection);
            }
            else
            {
                if (payLocation == null)
                {
                    subText.text = "Counter Not Found";
                    payLocation = mouvement.GoToPay(out lookDirection);
                }
                else
                {
                    if (mouvement.isAtLocation(payLocation.transform.position))
                    {
                        if (lookDirection == null)
                        {
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        }

                        etat = ClientState.Leaving;
                        ResourcesManager[] resources = FindObjectsOfType<ResourcesManager>();
                        for (int i = 0; i < resources.Length; i++)
                        {
                            resources[i].Gold += price;
                            resources[i].Reputation = isHappy ? 1 : -1;
                        }
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
            isHappy = true;
            hasBeenInteractedWith = false;
            price = (uint) Random.Range(orderPriceMin, orderPriceMax);
            hasAWaiter = false;
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