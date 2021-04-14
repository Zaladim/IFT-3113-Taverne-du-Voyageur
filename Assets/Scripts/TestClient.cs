using System;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestClient : MonoBehaviour
{
    public enum State
    {
        FindingSeat,
        WaitingToOrder,
        WaitingToReceiveOrder,
        Eating,
        WaitingToPay,
        Leaving,
        Inactive
    }

    private BasicAI movement;

    private TextMesh text;

    private TextMesh subText;

    private Seat seat;

    private Entrance exit;

    public State state;

    public float timeToEatMin = 5f;
    public float timeToEatMax = 20f;

    public float timeStateSwitchMin = 5f;
    public float timeStateSwitchMax = 20f;

    private float timeLeftToEat;
    private float timeLeftStateSwitch;


    // Start is called before the first frame update
    private void Start()
    {
        movement = GetComponent<BasicAI>();
        var temp = GetComponent<Transform>().Find("Text");
        text = temp.GetComponent<TextMesh>();
        temp = GetComponent<Transform>().Find("SubText");
        subText = temp.GetComponent<TextMesh>();
        state = State.FindingSeat;
        timeLeftStateSwitch = Random.Range(timeStateSwitchMin, timeStateSwitchMax);
        timeLeftToEat = timeLeftStateSwitch = Random.Range(timeToEatMin, timeToEatMax);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case State.FindingSeat:
                {
                    text.text = "Finding Seat";
                    if (seat == null)
                    {
                        subText.text = "Seat Not Found";
                        seat = movement.GoToRandomSeat();
                    }
                    else
                    {
                        subText.text = "Seat Found";
                        if (movement.IsAtLocation(seat.transform.position))
                        {
                            seat.isOccupied = true;
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            state = State.WaitingToOrder;
                        }
                    }

                    break;
                }
            case State.WaitingToOrder:
                {
                    text.text = "Waiting To Order";
                    subText.text = $"{Mathf.Ceil(timeLeftStateSwitch)}";
                    timeLeftStateSwitch -= Time.deltaTime;
                    if (timeLeftStateSwitch <= 0)
                    {
                        timeLeftStateSwitch =
                            timeLeftStateSwitch = Random.Range(timeStateSwitchMin, timeStateSwitchMax);
                        ;
                        state = State.WaitingToReceiveOrder;
                    }

                    break;
                }
            case State.WaitingToReceiveOrder:
                {
                    text.text = "Waiting To Receive Order";
                    subText.text = $"{Mathf.Ceil(timeLeftStateSwitch)}";
                    timeLeftStateSwitch -= Time.deltaTime;
                    if (timeLeftStateSwitch <= 0)
                    {
                        timeLeftStateSwitch =
                            timeLeftStateSwitch = Random.Range(timeStateSwitchMin, timeStateSwitchMax);
                        ;
                        state = State.Eating;
                        timeLeftToEat = timeLeftStateSwitch = Random.Range(timeToEatMin, timeToEatMax);
                    }

                    break;
                }
            case State.Eating:
                {
                    text.text = "Eating";
                    subText.text = $"{Mathf.Ceil(timeLeftStateSwitch)}";
                    timeLeftToEat -= Time.deltaTime;
                    if (timeLeftToEat <= 0)
                    {
                        subText.text = $"{Mathf.Ceil(timeLeftStateSwitch)}";
                        timeLeftStateSwitch -= Time.deltaTime;
                        if (timeLeftStateSwitch <= 0)
                        {
                            timeLeftStateSwitch =
                                timeLeftStateSwitch = Random.Range(timeStateSwitchMin, timeStateSwitchMax);
                            ;
                            state = State.WaitingToPay;
                        }
                    }

                    break;
                }
            case State.WaitingToPay:
                {
                    text.text = "Waiting To Pay";
                    subText.text = $"{Mathf.Ceil(timeLeftStateSwitch)}";
                    timeLeftStateSwitch -= Time.deltaTime;
                    if (timeLeftStateSwitch <= 0)
                    {
                        timeLeftStateSwitch =
                            timeLeftStateSwitch = Random.Range(timeStateSwitchMin, timeStateSwitchMax);
                        ;
                        state = State.Leaving;
                    }

                    break;
                }
            case State.Leaving:
                {
                    text.text = "Leaving";
                    if (seat != null)
                    {
                        subText.text = "Exit Not Found";
                        seat.isAIGoingForIt = false;
                        seat.isOccupied = false;
                        seat = null;
                        exit = movement.GoToExit();
                    }
                    else
                    {
                        if (exit == null)
                        {
                            subText.text = "Exit Not Found";
                            exit = movement.GoToExit();
                        }
                        else
                        {
                            subText.text = "Exit Found";
                            if (movement.IsAtLocation(exit.transform.position))
                            {
                                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                state = State.Inactive;
                            }
                        }
                    }

                    break;
                }
            case State.Inactive:
                {
                    text.text = "Inactive";
                    subText.text = $"{Mathf.Ceil(timeLeftStateSwitch)}";
                    timeLeftStateSwitch -= Time.deltaTime;
                    if (timeLeftStateSwitch <= 0)
                    {
                        timeLeftStateSwitch =
                            timeLeftStateSwitch = Random.Range(timeStateSwitchMin, timeStateSwitchMax);
                        state = State.FindingSeat;
                    }

                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}