using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomShadeBehavior : MonoBehaviour
{
    private int AngleRoom = 0;
    private GameObject Room;
    // Start is called before the first frame update
    void Start()
    {
        Room = GameObject.Find("Door");

    }

    // Update is called once per frame
    void Update()
    {
        AngleRoom = (int)Room.transform.localEulerAngles.y % 360;
        GameObject.FindWithTag(AngleRoom.ToString()).GetComponent<MeshRenderer>().enabled = true;
    }
}
