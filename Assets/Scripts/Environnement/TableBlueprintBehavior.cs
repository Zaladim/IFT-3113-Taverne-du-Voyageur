using System;
using System.Collections;
using System.Collections.Generic;
using Prototypes.Pathfinding.Scripts;
using UnityEngine;

public class TableBlueprintBehavior : MonoBehaviour
{
    private float rotateSpeed = 50f;
    [SerializeField] private Quaternion rot;

    private RaycastHit hit;
    private PlacementManager placementManager;

    [SerializeField] private GameObject prefab;
    private Graph graph;
    private List<Collider> colliders = new List<Collider>();

    private void Awake()
    {
        placementManager = GameObject.FindGameObjectWithTag("PlacementManager").GetComponent<PlacementManager>();
        graph = GameObject.Find("Graph").GetComponent<Graph>();
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rotateInput = Input.GetAxis("RotatePrefab");

        rot = transform.rotation;
        transform.Rotate(0.0f, rotateInput * rotateSpeed * Time.deltaTime, 0.0f);
        // rot.y += rotateInput * rotateSpeed * Time.deltaTime;
        // if (rot.y >= 1f)
        // {
        //     rot.y -= 1f;
        // }
        // if (rot.y <= -1f)
        // {
        //     rot.y += 1f;
        // }
        // transform.rotation = rot;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                Vector3 pos = hit.point;
                pos.y = hit.collider.transform.position.y + 1;
                transform.position = pos;
                
                //transform.position.y = hit.collider.transform.position.y + 1;

            }
        }

       

        if (Input.GetMouseButtonDown(0))
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(1f, 0.5f, 1f), transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.transform.IsChildOf(transform))
                {
                    print(hitCollider);
                    if (!hitCollider.gameObject.CompareTag("Ground"))
                    {
                        return;
                    }
                }
            }
            GameObject o = Instantiate(prefab, transform.position, transform.rotation);
            graph.UpdateGraph();
            Destroy(gameObject);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0.5f, 1f)*2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!colliders.Contains(other))
        {
            colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(colliders.Contains(other))
        {
            colliders.Remove(other);
        }
    }
}
