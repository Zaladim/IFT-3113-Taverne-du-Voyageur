using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{
    [SerializeField]private GameObject room;

    [SerializeField]private List<GameObject> walls = new List<GameObject>();

    private void Awake()
    {
        foreach (var wall in GameObject.FindGameObjectsWithTag("BorderWall"))
        {
            walls.Add(wall);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) //Raycast Collision. If clicked item is Borderwall, replace with Door
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("BorderWall"))
                {
                    walls.Remove(hit.collider.gameObject);
                    Transform tmp = hit.collider.gameObject.transform;
                    GameObject r = Instantiate(room);
                    r.transform.position = tmp.position;
                    r.transform.rotation = tmp.rotation;
                    foreach (var item in r.GetComponentsInChildren<Transform>())
                    {
                        if (item.CompareTag("BorderWall"))
                        {
                            if (CheckForOtherRoom(1f, item.gameObject))
                            {
                                Destroy(r);
                                return;
                            }
                        }
                    }
                    
                    foreach (var item in r.GetComponentsInChildren<Transform>())
                    {
                        if (item.CompareTag("BorderWall"))
                        {
                            DestroyObjectAtLocation(1f, item.gameObject);
                        }
                        
                    }
                    //Instantiate(room, tmp.position, tmp.rotation);
                    //DeleteExisting(r);
                    Destroy(hit.collider.gameObject);
                    //Destroy(r);
                }
            }
        }
    }

    private void DestroyObjectAtLocation(float minDist, GameObject item)
    {
        Vector3 tmpLocation = item.transform.position;
        // print(tmpLocation);
        Transform[] tiles = GameObject.FindObjectsOfType<Transform> ();
       
        for (int i = 0; i < tiles.Length; i++) {
            if(Vector3.Distance(tiles[i].position, tmpLocation) <= minDist){
                if (item != tiles[i].gameObject)
                {
                    item.tag = "Wall";
                    Destroy(tiles[i].gameObject);
                }
            }
        }
    }

    private bool CheckForOtherRoom(float minDist, GameObject item)
    {
        Vector3 tmpLocation = item.transform.position;
        // print(tmpLocation);
        Transform[] tiles = GameObject.FindObjectsOfType<Transform> ();
        
        for (int i = 0; i < tiles.Length; i++) {
            if(Vector3.Distance(tiles[i].position, tmpLocation) <= minDist){
                if (tiles[i].gameObject.CompareTag("Room"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
