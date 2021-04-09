﻿using Managers;
using UnityEngine;

namespace Environnement
{
    public class RoomBlueprintBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        private float delay;
        private RaycastHit hit;
        private PlacementManager placementManager;
        private readonly float tavernLimit = -10;
        [SerializeField]private GameObject wallAnchor;
        [SerializeField]private GameObject nextAnchor;

        private bool placeable = false;
        private void Awake()
        {
            placementManager = GameObject.FindGameObjectWithTag("PlacementManager").GetComponent<PlacementManager>();
        }

        private void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
                if (hit.collider.gameObject.CompareTag("BorderWall"))
                {
                    if (nextAnchor == hit.collider.gameObject)
                    {
                        delay += Time.deltaTime;
                    }
                    else
                    {
                        nextAnchor = hit.collider.gameObject;
                        delay = 0f;
                    }

                    if (delay >= 0.5f)
                    {
                        placeable = true;
                        wallAnchor = nextAnchor;
                        transform.position = hit.collider.transform.position;
                        transform.rotation = hit.collider.transform.rotation;
                    }
                }
            Collider[] hitColliders = Physics.OverlapBox(CalculateCentroid(), new Vector3(1.5f, 2f, 1.5f), transform.rotation);
            if (hitColliders.Length != 0)
            {
                placeable = false;
            }


            if (Input.GetMouseButtonDown(0) && placeable)
            {
                var o = Instantiate(prefab, transform.position, transform.rotation);
                gameObject.SetActive(false);

                foreach (var item in o.GetComponentsInChildren<Transform>())
                    if (item.CompareTag("BorderWall"))
                        if (CheckForOtherRoom(1f, item.gameObject))
                        {
                            Destroy(o);
                            gameObject.SetActive(true);
                            return;
                        }

                foreach (var item in o.GetComponentsInChildren<Transform>())
                    if (item.CompareTag("BorderWall"))
                    {
                        if (item.position.x < tavernLimit) item.gameObject.tag = "Wall";

                        DestroyObjectAtLocation(0.5f, item.gameObject);
                    }

                gameObject.SetActive(true);
                placementManager.InitAllNodes();
                Destroy(wallAnchor);
                Destroy(gameObject);
            }
        }

        private void DestroyObjectAtLocation(float minDist, GameObject item)
        {
            var tmpLocation = item.transform.position;

            //Transform[] tiles = GameObject.FindObjectsOfType<Transform> ();
            var tiles =
                Physics.OverlapBox(item.transform.position, new Vector3(1f, 1f, 1f) / 2, item.transform.rotation);
            for (var i = 0; i < tiles.Length; i++)
            {
                if (item.GetComponent<Collider>() == tiles[i]) continue;

                if (Vector3.Distance(tiles[i].transform.position, tmpLocation) <= minDist)
                    if (item != tiles[i].gameObject)
                    {
                        item.tag = "Wall";
                        Destroy(tiles[i].gameObject);
                    }
            }
        }

        private bool CheckForOtherRoom(float minDist, GameObject item)
        {
            var tmpLocation = item.transform.position;
            // print(tmpLocation);
            var tiles = FindObjectsOfType<Transform>();

            for (var i = 0; i < tiles.Length; i++)
                if (Vector3.Distance(tiles[i].position, tmpLocation) <= minDist)
                    if (tiles[i].gameObject.CompareTag("Room"))
                        return true;
            Collider[] hitColliders =
                Physics.OverlapBox(transform.position, new Vector3(2f, 2f, 2f), transform.rotation);
            print(hitColliders.Length);
            
            return false;
        }

        private Vector3 CalculateCentroid()
        {
            Vector3 centroid = Vector3.zero;
            if (transform.root.gameObject == transform.gameObject)
            {
                
                if (transform.childCount > 0)
                {
                    Transform[] allChilds = transform.gameObject.GetComponentsInChildren<Transform>();
                    foreach (var child in allChilds)
                    {
                        centroid += child.transform.position;
                    }

                    centroid /= allChilds.Length;
                }
                
            }
            return centroid;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.matrix = Matrix4x4.TRS(CalculateCentroid(), transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(2f, 2f, 2f)*2);
        }
    }
}