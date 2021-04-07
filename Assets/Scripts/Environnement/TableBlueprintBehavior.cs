using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Environnement
{
    public class TableBlueprintBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int seats;

        private ResourcesManager resourcesManager;
        private float rotateSpeed = 50f;
        private RaycastHit hit;
        private Graph graph;
        private List<Collider> colliders = new List<Collider>();

        private void Awake()
        {
            graph = GameObject.Find("Graph").GetComponent<Graph>();
            resourcesManager = FindObjectOfType<ResourcesManager>();
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rotateInput = Input.GetAxis("RotatePrefab");

            transform.Rotate(0.0f, rotateInput * rotateSpeed * Time.deltaTime, 0.0f);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    Vector3 pos = hit.point;
                    pos.y = hit.collider.transform.position.y + 1;
                    transform.position = pos;
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                var hitColliders =
                    Physics.OverlapBox(transform.position, new Vector3(1f, 0.5f, 1f), transform.rotation);
                if (hitColliders.Where(hitCollider => !hitCollider.transform.IsChildOf(transform))
                    .Any(hitCollider => !hitCollider.gameObject.CompareTag("Ground")))
                {
                    return;
                }

                Instantiate(prefab, transform.position, transform.rotation);
                resourcesManager.Seats = seats;
                graph.UpdateGraph();
                Destroy(gameObject);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0.5f, 1f) * 2);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!colliders.Contains(other))
            {
                colliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (colliders.Contains(other))
            {
                colliders.Remove(other);
            }
        }
    }
}