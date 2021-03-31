using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBlueprintBehavior : MonoBehaviour
{
    private RaycastHit hit;

    [SerializeField] private GameObject prefab;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
            gameObject.SetActive(false);
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, 1f);
            
            GameObject o = Instantiate(prefab, transform.position, Quaternion.identity);
            foreach (var hitCollider in hitColliders)
            {
                //print(hitCollider);
                if (!hitCollider.gameObject.CompareTag("Ground"))
                {
                    gameObject.SetActive(true);
                    Destroy(o);
                    return;
                }
            }
            
            
            
            gameObject.SetActive(true);
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
