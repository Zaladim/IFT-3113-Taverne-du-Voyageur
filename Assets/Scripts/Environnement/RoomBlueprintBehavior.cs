using System;
using System.Collections;
using System.Collections.Generic;
using Prototypes.Pathfinding.Scripts;
using UnityEngine;

public class RoomBlueprintBehavior : MonoBehaviour
{
    private float tavernLimit = -10;
    private RaycastHit hit;
    private GameObject wallAnchor;

    [SerializeField] private GameObject prefab;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("BorderWall"))
            {
                wallAnchor = hit.collider.gameObject;
                transform.position = hit.collider.transform.position;
                transform.rotation = hit.collider.transform.rotation;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
            
            foreach (var item in o.GetComponentsInChildren<Transform>())
            {
                if (item.CompareTag("BorderWall"))
                {
                    if (CheckForOtherRoom(1f, item.gameObject))
                    {
                        Destroy(o);
                        gameObject.SetActive(true);
                        return;
                    }
                }
            }

            foreach (var item in o.GetComponentsInChildren<Transform>())
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
            
            gameObject.SetActive(true);
            Destroy(wallAnchor);
            Destroy(gameObject);
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
