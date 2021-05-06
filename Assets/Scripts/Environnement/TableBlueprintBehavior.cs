using System.Collections.Generic;
using Managers;
using Pathfinding;
using UnityEngine;

namespace Environnement
{
    public class TableBlueprintBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int seats;
        [SerializeField] private Material defaultMat;
        [SerializeField] private Material constructMat;


        [SerializeField] private List<GameObject> childs = new List<GameObject>();
        private GameManager gameManager;

        [SerializeField] private Vector3 pos;
        [SerializeField] private Collider tmp;
        private readonly List<Collider> colliders = new List<Collider>();
        private readonly float rotateSpeed = 50f;
        private Graph graph;
        private RaycastHit hit;

        private ResourcesManager resourcesManager;
        private Grid grid;

        public int tableLength;
        public int tableWidth;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            graph = GameObject.Find("Graph").GetComponent<Graph>();
            resourcesManager = FindObjectOfType<ResourcesManager>();
            grid = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>().Grid;
        }

        private void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var rotateInput = Input.GetAxisRaw("RotatePrefab");
            var elapsedTime = Time.unscaledDeltaTime;

            transform.Rotate(0.0f, rotateInput * rotateSpeed * elapsedTime, 0.0f);

            if (Physics.Raycast(ray, out hit))
            {
                tmp = hit.collider;
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    pos = hit.point;
                    pos.y += 1;
                    Vector3 cell = grid.getCellPosition(pos);
                    pos.x = cell.x;
                    pos.z = cell.z;

                    transform.position = pos;

                    // var hitColliders =
                    //     Physics.OverlapBox(transform.position, new Vector3(1f, 0.5f, 2f), transform.rotation);
                    // foreach (var hitCollider in hitColliders)
                    //     if (!hitCollider.transform.IsChildOf(transform))
                    //         if (!hitCollider.gameObject.CompareTag("Ground"))
                    //         {
                    //             setColor(true);
                    //             return;
                    //         }

                    if (!checkPosition())
                    {
                        setColor(true);
                        return;
                    }

                    setColor(false);
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                var transform1 = transform;

                if (!checkPosition())
                {
                    return;
                }

                updateGrid();


                // var hitColliders =
                //     Physics.OverlapBox(transform1.position, new Vector3(1f, 0.5f, 2f), transform1.rotation);
                // foreach (var hitCollider in hitColliders)
                //     if (!hitCollider.transform.IsChildOf(transform))
                //         if (!hitCollider.gameObject.CompareTag("Ground"))
                //             return;

                Instantiate(prefab, transform1.position - new Vector3(0f, 0.5f, 0f), transform1.rotation);
                resourcesManager.Seats += seats;
                graph.UpdateGraph();
                Destroy(gameObject);
                gameManager.GamePause = false;
            }
        }

        private bool checkPosition()
        {
            int x, y;
            grid.getXY(transform.position, out x, out y);

            for (int i = x - tableWidth; i <= x + tableWidth; i++)
            {
                for (int j = y - tableLength; j <= y + tableLength; j++)
                {
                    if (grid.GetValue(i, j) != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void updateGrid()
        {
            int x, y;
            grid.getXY(transform.position, out x, out y);

            for (int i = x - tableWidth; i <= x + tableWidth; i++)
            {
                for (int j = y - tableLength; j <= y + tableLength; j++)
                {
                    grid.SetValue(i, j, -1);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 0.5f, 2f) * 2);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!colliders.Contains(other)) colliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (colliders.Contains(other)) colliders.Remove(other);
        }

        private void setColor(bool state)
        {
            if (!state)
                foreach (var child in childs)
                    //Couleur non plaçable
                    child.GetComponent<Renderer>().material = defaultMat;
            else
                foreach (var child in childs)
                    //Couleur plaçable
                    child.GetComponent<Renderer>().material = constructMat;
        }
    }
}