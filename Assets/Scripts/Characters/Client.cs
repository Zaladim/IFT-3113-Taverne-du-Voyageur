using System.Linq;
using Interface;
using Managers;
using Pathfinding;
using UnityEngine;

namespace Characters
{
    public enum ClientState
    {
        FindingSeat,
        LookingAround,
        WaitingToOrder,
        WaitingToReciveOrder,
        Eating,
        GoingToPay,
        GettingQuest,
        Idle,
        Leaving,
        Inactive
    }

    public class Client : MonoBehaviour
    {
        [Header("Props")] [SerializeField] private BasicAI mouvement;
        [SerializeField] private TextMesh text;
        [SerializeField] private TextMesh subText;
        [SerializeField] private ResourcesManager resourcesManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Animator anim;

        [Header("Options")] [SerializeField] private int orderPriceMin = 10;
        [SerializeField] private int orderPriceMax = 40;
        [SerializeField] private float timeToEatMin = 5f;
        [SerializeField] private float timeToEatMax = 10f;
        [SerializeField] private float timeToRecieveOrderMin = 10f;
        [SerializeField] private float timeToRecieveOrderMax = 15f;
        [SerializeField] private float waitingTimeMin = 15f;
        [SerializeField] private float waitingTimeMax = 20f;
        [Space] [SerializeField] private int unHappyTimeMax = -10;
        [SerializeField] private int unHappyPrice;
        [SerializeField] private int notServedPrice;
        [SerializeField] private int notServedTimeMax = -25;
        [SerializeField] private int notServedTime;
        [SerializeField] private int unHappyTime;
        [SerializeField] private int happyReputation = 1;
        [SerializeField] private int unHappyReputation = -1;
        [SerializeField] private int notServedReputation = -3;
        [SerializeField] private float distanceFromQuestGiverToInteract = 2;
        [SerializeField] private float timeToStayIdleMin = 1f;
        [SerializeField] private float timeToStayIdleMax = 5f;
        [SerializeField] private float timeToWaitBeforeLookingForDestinationAgain = 1f;
        [SerializeField] private Node currentLookingAroundNode;
        [SerializeField] private int nbLookingAroundNodes = 10;
        [SerializeField] private int lookingAroundNodesLeft;


        [Header("Debug")] [SerializeField] private ClientState etat;
        [SerializeField] private Entrance exit;
        [SerializeField] private bool hasBeenInteractedWith;
        [SerializeField] private bool isHappy = true;
        [SerializeField] private GameObject lookDirection;
        [SerializeField] private GameObject payLocation;
        [SerializeField] private int price;
        [SerializeField] private Seat seat;
        [SerializeField] private GameObject questGiver;
        [SerializeField] private float timer;
        [SerializeField] private float newDestinationTimer;

        [SerializeField] private bool hasQuest;

        public bool HasAWaiter { get; set; }

        public float TimeLeft { get; private set; }

        public ResourcesManager ResourcesManager
        {
            get => resourcesManager;
            set => resourcesManager = value;
        }

        public GameManager GameManager
        {
            set => gameManager = value;
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
                        anim.SetBool("moving", true);
                        text.text = "Finding Seat";
                        if (seat == null)
                        {
                            subText.text = "Seat Not Found";
                            if (newDestinationTimer == -1)
                            {
                                etat = ClientState.LookingAround;
                                lookingAroundNodesLeft = nbLookingAroundNodes;
                            }
                            else
                            {
                                newDestinationTimer = -1;
                                seat = mouvement.GoToRandomSeat();
                                if (seat != null) lookDirection = seat.lookDirection;
                            }
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
                            else if (mouvement.CheckIfPathNotFound())
                            {
                                newDestinationTimer = 0;
                                seat.isOccupied = false;
                                seat.isAIGoingForIt = false;
                                seat = null;
                            }
                        }

                        break;
                    }
                case ClientState.LookingAround:
                    {
                        anim.SetBool("moving", true);
                        text.text = "Looking Around";
                        subText.text = "nb Nodes left: " + lookingAroundNodesLeft;
                        if (lookingAroundNodesLeft >= 0)
                        {
                            if (currentLookingAroundNode == null)
                            {
                                if (newDestinationTimer > 0)
                                {
                                    newDestinationTimer -= Time.deltaTime;
                                }
                                else
                                {
                                    var allSeats = FindObjectsOfType<Seat>();

                                    var areAllSeatsOccupied = allSeats.All(t => t.isAIGoingForIt);
                                    if (!areAllSeatsOccupied)
                                    {
                                        etat = ClientState.FindingSeat;
                                    }
                                    else
                                    {
                                        newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                                        currentLookingAroundNode = mouvement.GoToRandomLookAroundNode();
                                    }
                                }
                            }
                            else
                            {
                                if (mouvement.IsAtLocation(currentLookingAroundNode.getPosition()))
                                {
                                    lookingAroundNodesLeft -= 1;
                                    currentLookingAroundNode = null;
                                }
                                else if (mouvement.CheckIfPathNotFound())
                                {
                                    currentLookingAroundNode = null;
                                }
                            }
                        }
                        else
                        {
                            var allSeats = FindObjectsOfType<Seat>();

                            var areAllSeatsOccupied = allSeats.All(t => t.isAIGoingForIt);
                            if (!areAllSeatsOccupied)
                                etat = ClientState.FindingSeat;
                            else
                            {
                                unHappyReputation = -2;
                                unHappyPrice = 0;
                                isHappy = false;
                                etat = ClientState.GoingToPay;
                            }
                        }

                        break;
                    }
                case ClientState.WaitingToOrder:
                    anim.SetBool("moving", false);
                    timer -= Time.deltaTime;

                    text.text = "Waiting To Order";
                    subText.text = Mathf.Ceil(timer) + "s";

                    if (timer <= 0)
                    {
                        isHappy = false;
                        etat = ClientState.GoingToPay;
                    }

                    if (hasBeenInteractedWith)
                    {
                        etat = ClientState.WaitingToReciveOrder;
                        TimeLeft = Random.Range(timeToRecieveOrderMin, timeToRecieveOrderMax);
                        hasBeenInteractedWith = false;
                    }

                    if (lookDirection != null) mouvement.FaceLocation(lookDirection.transform.position);

                    break;
                case ClientState.WaitingToReciveOrder:
                    {
                        anim.SetBool("moving", false);
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
                        anim.SetBool("moving", false);
                        text.text = "Eating";
                        subText.text = Mathf.Ceil(timer) + "s";
                        timer -= Time.deltaTime;
                        if (timer <= 0) etat = ClientState.GoingToPay;

                        if (lookDirection != null) mouvement.FaceLocation(lookDirection.transform.position);

                        break;
                    }
                case ClientState.GoingToPay:
                    {
                        anim.SetBool("moving", true);
                        text.text = "Going To Pay";
                        if (seat != null)
                        {
                            subText.text = "Counter Not Found";
                            seat.isAIGoingForIt = false;
                            seat.isOccupied = false;
                            seat = null;
                            payLocation = mouvement.GoToPay(out lookDirection);
                            newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                        }
                        else
                        {
                            if (payLocation == null)
                            {
                                subText.text = "Counter Not Found";
                                if (newDestinationTimer > 0)
                                {
                                    newDestinationTimer -= Time.deltaTime;
                                }
                                else
                                {
                                    newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                                    payLocation = mouvement.GoToPay(out lookDirection);
                                }
                            }
                            else
                            {
                                subText.text = "Counter Found";
                                if (mouvement.IsAtLocation(payLocation.transform.position))
                                {
                                    if (lookDirection == null) transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                                    var g = isHappy ? price : unHappyPrice;
                                    var r = isHappy ? happyReputation : unHappyReputation;
                                    resourcesManager.Gold += g;
                                    resourcesManager.Reputation += r;

                                    if (!isHappy)
                                    {
                                        var gs = g > 0 ? $"{g} coins |" : "";
                                        var rs = $"{r} reputation";
                                        var tot = $"{gs} {rs}";

                                        gameManager.NotificationSystem.CreateNotification(
                                            $"Oohh... Noo...\nIt seems that you get an unhappy client!\n{tot}",
                                            4f,
                                            NotificationType.Danger);
                                    }

                                    etat = ClientState.GettingQuest;
                                }
                                else if (mouvement.CheckIfPathNotFound())
                                {
                                    payLocation = null;
                                }
                            }
                        }

                        break;
                    }
                case ClientState.GettingQuest:
                    {
                        anim.SetBool("moving", true);
                        text.text = "Getting Quest";
                        if (payLocation != null)
                        {
                            subText.text = "Quest Giver Not Found";
                            payLocation = null;
                            if (questGiver == null)
                                questGiver = mouvement.GoToRandomQuestGiver();
                            else
                                mouvement.GoToQuestGiver(questGiver);
                        }
                        else
                        {
                            if (questGiver == null)
                            {
                                etat = ClientState.Leaving;
                            }
                            else
                            {
                                subText.text = "Quest Giver Found";
                                if (mouvement.IsCloseToLocation(questGiver.transform.position,
                                    distanceFromQuestGiverToInteract))
                                {
                                    etat = ClientState.Idle;
                                    timer = Random.Range(timeToStayIdleMin, timeToStayIdleMax);
                                    if (hasQuest)
                                    {
                                        questGiver.GetComponent<QuestGiver>().ReturnQuest();
                                        hasQuest = false;
                                        exit = null;
                                    }
                                    else
                                    {
                                        hasQuest = true;
                                    }
                                }
                                else if (mouvement.CheckIfPathNotFound())
                                {
                                    questGiver = null;
                                }
                            }
                        }


                        break;
                    }
                case ClientState.Idle:
                    {
                        anim.SetBool("moving", false);
                        mouvement.StopMoving();
                        mouvement.FaceLocation(questGiver.transform.position);
                        text.text = "Talking To Quest Giver";
                        subText.text = "Time Left: " + Mathf.Ceil(timer) + "s";
                        timer -= Time.deltaTime;

                        if (timer <= 0)
                        {
                            etat = ClientState.Leaving;

                            if (!hasQuest) questGiver = null;
                        }

                        break;
                    }
                case ClientState.Leaving:
                    {
                        anim.SetBool("moving", true);
                        text.text = "Leaving";

                        if (payLocation != null)
                        {
                            subText.text = "Exit Not Found";
                            payLocation = null;
                            exit = mouvement.GoToExit();
                            newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                        }
                        else
                        {
                            if (exit == null)
                            {
                                subText.text = "Exit Not Found";
                                if (newDestinationTimer > 0)
                                {
                                    newDestinationTimer -= Time.deltaTime;
                                }
                                else
                                {
                                    newDestinationTimer = timeToWaitBeforeLookingForDestinationAgain;
                                    exit = mouvement.GoToExit();
                                }
                            }
                            else
                            {
                                subText.text = "Exit Found";
                                if (mouvement.IsAtLocation(exit.transform.position))
                                {
                                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                    etat = ClientState.Inactive;
                                }
                                else if (mouvement.CheckIfPathNotFound())
                                {
                                    exit = null;
                                }
                            }
                        }

                        break;
                    }
                case ClientState.Inactive:
                    SetUp();
                    gameObject.SetActive(false);
                    exit = null;
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
            TimeLeft = Random.Range(timeToRecieveOrderMin, timeToRecieveOrderMax);
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