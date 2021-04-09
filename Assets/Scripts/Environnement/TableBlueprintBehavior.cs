using System.Collections.Generic;
using System.Linq;
using Managers;
using Pathfinding;
using UnityEngine;

namespace Environnement
{
    public class TableBlueprintBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int seats;
        [SerializeField] private Material DefaultMat;
        [SerializeField] private Material ConstructMat;

        private ResourcesManager resourcesManager;
        private float rotateSpeed = 50f;
        private RaycastHit hit;
        private Graph graph;
        private List<Collider> colliders = new List<Collider>();
        
        [SerializeField] private List<GameObject> childs = new List<GameObject>();

        [SerializeField] private Vector3 pos;
        [SerializeField] private Collider tmp;

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
                tmp = hit.collider;
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    pos = hit.point;
                    pos.y += 1;
                    transform.position = pos;
                    
                    Collider[] hitColliders =
                        Physics.OverlapBox(transform.position, new Vector3(1f, 0.5f, 2f), transform.rotation);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (!hitCollider.transform.IsChildOf(transform))
                        {
                            if (!hitCollider.gameObject.CompareTag("Ground"))
                            {
                                setColor(true);
                                return;
                            }
                        }
                    }

                    setColor(false);
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                Collider[] hitColliders =
                    Physics.OverlapBox(transform.position, new Vector3(1f, 0.5f, 2f), transform.rotation);
                foreach (var hitCollider in hitColliders)
                {
                    if (!hitCollider.transform.IsChildOf(transform))
                    {
                        if (!hitCollider.gameObject.CompareTag("Ground"))
                        {
                            Debug.Log("Placement Impossible!");
                            return;
                        }
                    }
                }

                Instantiate(prefab, transform.position, transform.rotation);
                resourcesManager.Seats += seats;
                graph.UpdateGraph();
                Destroy(gameObject);
            }
        }

        void setColor(bool state)
        {
            Material defaultMat =  null;

            Collider[] hitColliders =
                Physics.OverlapBox(transform.position, new Vector3(1f, 0.5f, 2f), transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.transform.IsChildOf(transform))
                {
                    if (!hitCollider.gameObject.CompareTag("Ground"))
                    {

                        foreach (var child in childs)
                        {
                            if(child.gameObject.tag == "Model"){
                                //Changer a couleur du gameobject

                                Renderer render = child.GetComponent<Renderer>();
                                defaultMat = render.material;

                                render.material = ConstructMat;

                            }
                        }
                        return;
                    }
                }
            }
            if(!state){
                foreach (var child in childs)
                {
                  //Retirer la couleur du Gameobject
                  child.GetComponent<Renderer>().material = defaultMat;

                }
            }
            
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 0.5f, 2f)*2);
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