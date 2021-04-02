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
        private GameObject wallAnchor;


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
                    if (wallAnchor == hit.collider.gameObject)
                    {
                        delay += Time.deltaTime;
                    }
                    else
                    {
                        wallAnchor = hit.collider.gameObject;
                        delay = 0f;
                    }

                    if (delay >= 0.5f)
                    {
                        transform.position = hit.collider.transform.position;
                        transform.rotation = hit.collider.transform.rotation;
                    }
                }

            if (Input.GetMouseButtonDown(0))
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

            return false;
        }
    }
}