using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Placement : MonoBehaviour
{
    [SerializeField] private float tavernLimit = -10;

    [SerializeField]private GameObject room;
    [SerializeField]private GameObject table;

    [SerializeField]private List<GameObject> walls = new List<GameObject>();

    [SerializeField] private GameObject roomUI;
    
    public bool placeRoom;

    public GameObject Room
    {
        get => room;
        set => room = value;
    }

    private void Awake()
    {
        foreach (var wall in GameObject.FindGameObjectsWithTag("BorderWall"))
        {
            if (wall.transform.position.x < tavernLimit)
            {
                wall.tag = "Wall";
            }
            else
            {
                walls.Add(wall);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) //Raycast Collision. If clicked item is Borderwall, replace with Door
        {
            if (placeRoom)
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
                                if (item.position.x < tavernLimit)
                                {
                                    item.gameObject.tag = "Wall";
                                }

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
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("Ground"))
                    {
                        Collider[] hitColliders = Physics.OverlapSphere(hit.point, 2f);
                        foreach (var hitCollider in hitColliders)
                        {
                            //print(hitCollider);
                            if (!hitCollider.gameObject.CompareTag("Ground"))
                            {
                                return;
                            }
                        }
                        Instantiate(table, hit.point, Quaternion.identity);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (roomUI.gameObject.activeSelf)
            {
                roomUI.gameObject.SetActive(false);
            } else
            {
                roomUI.gameObject.SetActive(true);
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
