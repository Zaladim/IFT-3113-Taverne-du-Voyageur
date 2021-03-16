using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototypes.Pathfinding.Scripts
{
    public class Node : MonoBehaviour, IEquatable<Node>
    {
        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(neighborsNodes, other.neighborsNodes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (neighborsNodes != null ? neighborsNodes.GetHashCode() : 0);
            }
        }
        //public List<GameObject> neighborsObjects;

        private List<Node> neighborsNodes;

        // Start is called before the first frame update
        private void Start()
        {
            initialize();
        }

        public void initialize()
        {
            neighborsNodes = new List<Node>();
            var allNodes = FindObjectsOfType<Node>();
            foreach (var t in allNodes)
            {
                if (t == this) continue;
                if (canReachOtherNode(t))
                {
                    neighborsNodes.Add(t);
                }
            }
        }

        private bool canReachOtherNode(Component node)
        {
            var direction = node.transform.position - getPosition();
            var hits = new RaycastHit[] { };
            Physics.RaycastNonAlloc(getPosition(), direction, hits);
            var canGoToObject =
                (from t in hits
                    where t.collider.GetComponent<ObjectAIBehavior>() != null
                    select t.collider.GetComponent<ObjectAIBehavior>()).All(behavior => behavior.canActorsPassThrough);
            return canGoToObject;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public Vector3 getPosition()
        {
            return transform.position;
        }

        public IEnumerable<Node> getNeighbors()
        {
            return neighborsNodes;
        }

        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }
    }
}