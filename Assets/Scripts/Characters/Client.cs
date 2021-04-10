using Managers;
using Pathfinding;
using UnityEngine;

namespace Characters
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

    public class Client : MonoBehaviour
    {
        [Header("Props")] [SerializeField] private BasicAI mouvement;
        [SerializeField] private TextMesh text;
        [SerializeField] private TextMesh subText;
        [SerializeField] private ResourcesManager resourcesManager;

        [Header("Options")] [SerializeField] private int orderPriceMin = 10;
        [SerializeField] private int orderPriceMax = 40;
        [SerializeField] private float timeToEatMin = 5f;
        [SerializeField] private float timeToEatMax = 20f;
        [SerializeField] private float timeToOrderMin = 10f;
        [SerializeField] private float timeToOrderMax = 100f;
        [SerializeField] private float waitingTimeMin = 45f;
        [SerializeField] private float waitingTimeMax = 100f;
        [Space] [SerializeField] private int unHappyTimeMax = -10;
        [SerializeField] private int unHappyPrice;
        [SerializeField] private int notServedPrice;
        [SerializeField] private int notServedTimeMax = -25;
        [SerializeField] private int notServedTime;
        [SerializeField] private int unHappyTime;
        [SerializeField] private int happyReputation = 1;
        [SerializeField] private int unHappyReputation = -1;
        [SerializeField] private int notServedReputation = -3;

        [Header("Debug")] [SerializeField] private ClientState etat;
        [SerializeField] private Entrance exit;
        [SerializeField] private bool hasBeenInteractedWith;
        [SerializeField] private bool isHappy = true;
        [SerializeField] private GameObject lookDirection;
        [SerializeField] private GameObject payLocation;
        [SerializeField] private int price;
        [SerializeField] private Seat seat;
        [SerializeField] private float timer;

        public bool HasAWaiter { get; set; }

        public float TimeLeft { get; private set; }

        public ResourcesManager ResourcesManager
        {
            get => resourcesManager;
            set => resourcesManager = value;
        }

        // Start is called before the first frame update
        private void Start()
        {
            SetUp();
        }

        // Update is called once per frame
        private void Update()
        {
            switch (etat)
            {
                case ClientState.FindingSeat:
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
                        if (mouvement.IsAtLocation(seat.transform.position))
                        {
                            seat.isOccupied = true;
                            if (lookDirection == null) transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                            timer = Random.Range(waitingTimeMin, waitingTimeMax);
                            etat = ClientState.WaitingToOrder;
                        }
                    }

                    break;
                }
                case ClientState.WaitingToOrder:
                    timer -= Time.deltaTime;

                    text.text = "Waiting To Order";
                    subText.text = Mathf.Ceil(timer) + "s";

                    if (timer <= 0)
                    {
                        unHappyReputation = notServedReputation;
                        unHappyPrice = notServedPrice;
                        isHappy = false;
                        etat = ClientState.GoingToPay;
                    }

                    if (hasBeenInteractedWith)
                    {
                        etat = ClientState.WaitingToReciveOrder;
                        TimeLeft = Random.Range(timeToOrderMin, timeToOrderMax);
                        hasBeenInteractedWith = false;
                    }

                    if (lookDirection != null) mouvement.FaceLocation(lookDirection.transform.position);

                    break;
                case ClientState.WaitingToReciveOrder:
                {
                    TimeLeft -= Time.deltaTime;
                    isHappy = TimeLeft >= unHappyTime;

                    if (TimeLeft < notServedTime)
                    {
                        unHappyReputation = notServedReputation;
                        unHappyPrice = notServedPrice;
                        isHappy = false;
                        etat = ClientState.GoingToPay;
                    }

                    text.text = "Waiting To Recive Order";
                    subText.text = Mathf.Ceil(TimeLeft) + "s";

                    if (hasBeenInteractedWith)
                    {
                        etat = ClientState.Eating;
                        timer = Random.Range(timeToEatMin, timeToEatMax);
                        hasBeenInteractedWith = false;
                    }

                    if (lookDirection != null) mouvement.FaceLocation(lookDirection.transform.position);

                    break;
                }
                case ClientState.Eating:
                {
                    text.text = "Eating";
                    subText.text = Mathf.Ceil(timer) + "s";
                    timer -= Time.deltaTime;
                    if (timer <= 0) etat = ClientState.GoingToPay;

                    if (lookDirection != null) mouvement.FaceLocation(lookDirection.transform.position);

                    break;
                }
                case ClientState.GoingToPay:
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
                            if (mouvement.IsAtLocation(payLocation.transform.position))
                            {
                                if (lookDirection == null) transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                                var g = isHappy ? price : unHappyPrice;
                                var r = isHappy ? happyReputation : unHappyReputation;
                                resourcesManager.Gold += g;
                                resourcesManager.Reputation += r;

                                etat = ClientState.Leaving;
                            }
                        }
                    }

                    break;
                }
                case ClientState.Leaving:
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
                            if (mouvement.IsAtLocation(exit.transform.position))
                            {
                                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                etat = ClientState.Inactive;
                            }
                        }
                    }

                    break;
                }
                case ClientState.Inactive:
                    SetUp();
                    gameObject.SetActive(false);
                    break;
            }
        }

        private void SetUp()
        {
            mouvement.Setup();
            hasBeenInteractedWith = false;
            HasAWaiter = false;
            isHappy = true;
            notServedTime = Random.Range(notServedTimeMax, 0);
            unHappyTime = Random.Range(unHappyTimeMax, 0);
            TimeLeft = Random.Range(timeToOrderMin, timeToOrderMax);
            price = Random.Range(orderPriceMin, orderPriceMax);
            etat = ClientState.FindingSeat;
        }

        public void InteractWIthClient()
        {
            hasBeenInteractedWith = true;
        }

        public ClientState GETState()
        {
            return etat;
        }

        public Vector3 GETPosition()
        {
            return mouvement.GETLocation();
        }
    }
}