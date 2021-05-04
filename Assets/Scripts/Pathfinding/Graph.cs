using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private List<Node> nodes = new List<Node>();

        public bool DrawGraph; //debug only

        private bool isAlive = true;

        private bool isOriginal = true;

        public void Kill()
        {
            isAlive = false;
        }

        public void Resurect()
        {
            isAlive = true;
        }

        public Graph CopyGraph()
        {
            Graph temp = gameObject.AddComponent<Graph>();
            temp.nodes = nodes;
            temp.DrawGraph = DrawGraph;
            temp.isOriginal = false;
            return temp;
        }

        // Start is called before the first frame update
        private void Start()
        {
            GameObject[] n = GameObject.FindGameObjectsWithTag("Node");
            foreach (var node in n)
            {
                nodes.Add(node.GetComponent<Node>());
            }

            // nodes = FindObjectsOfType<Node>();
            drawGraph(DrawGraph);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void UpdateGraph()
        {
            nodes.Clear();
            GameObject[] n = GameObject.FindGameObjectsWithTag("Node");
            foreach (var node in n)
            {
                nodes.Add(node.GetComponent<Node>());
            }

            // nodes = FindObjectsOfType<Node>();
            for (var i = 0; i < nodes.Count; i++) nodes[i].initialize();

            if (isOriginal)
            {
                //take care of all the copies of the graph
                BasicAI[] allBasicAI = FindObjectsOfType<BasicAI>();
                for (int i = 0; i < allBasicAI.Length; i++)
                {
                    allBasicAI[i].ScheduledGraphUpdate();
                }
            }
        }

        public List<Node> getNodes()
        {
            return nodes;
        }

        public void drawGraph(bool inEditor)
        {
            if (inEditor)
            {
                nodes.Clear();
                GameObject[] n = GameObject.FindGameObjectsWithTag("Node");
                foreach (var node in n)
                {
                    nodes.Add(node.GetComponent<Node>());
                }
                // nodes = FindObjectsOfType<Node>();
            }

            foreach (var t in nodes) t.initialize();

            foreach (var t in nodes)
            {
                var position = t.getPosition();
                Debug.DrawLine(position - new Vector3(0.1f, 0.1f, 0.1f), position + new Vector3(0.1f, 0.1f, 0.1f),
                    Color.blue, 2);

                var neighbors = t.getNeighbors();

                foreach (var t1 in neighbors)
                {
                    var neighborsNeighbors = t1.getNeighbors();
                    foreach (var t2 in neighborsNeighbors)
                    {
                        if (t2 == t)
                        {
                            Debug.DrawLine(position, t1.getPosition(), Color.green, 2);
                            break;
                        }

                        Debug.DrawLine(position, t1.getPosition(), Color.red, 2);
                    }
                }
            }
        }


        public Node getColsestNodeToPoint(Vector3 position)
        {
            Node closestNode = null;
            var closestDistance = float.MaxValue;

            foreach (var t1 in nodes)
            {
                var direction = t1.getPosition() - position;
                var currentDistance = Vector3.Distance(t1.getPosition(), position);
                if (closestNode != null && !(currentDistance < closestDistance)) continue;
                var hits = Physics.RaycastAll(position, direction, currentDistance);
                var canGoToObject =
                    (from t in hits
                        where t.collider.GetComponent<ObjectAIBehavior>() != null
                        select t.collider.GetComponent<ObjectAIBehavior>())
                    .All(behavior => behavior.canActorsPassThrough);

                if (!canGoToObject) continue;
                closestNode = t1;
                closestDistance = currentDistance;
            }

            return closestNode;
        }

        public List<Node> A_Star(Node startNode, Node endNode)
        {
            /*var startNode = getColsestNodeToPoint(start);
            var endNode = getColsestNodeToPoint(end);*/

            // The set of currently discovered nodes that are not evaluated yet.
            // Initially, only the start node is known.
            var openSet = new List<Node> {startNode};

            var vistedNodes = new bool[nodes.Count];

            // For each node, which node it can most efficiently be reached from.
            // If a node can be reached from many nodes, cameFrom will eventually contain the
            // most efficient previous step.
            var cameFrom = new Node[nodes.Count];

            // For each node, the cost of getting from the start node to that node.
            var gScore = new float[nodes.Count];

            // For each node, the total cost of getting from the start node to the goal
            // by passing by that node. That value is partly known, partly heuristic.
            var fScore = new float[nodes.Count];
            //initialize all the lists
            for (var i = 0; i < nodes.Count; i++)
            {
                if (endNode is null) continue;
                cameFrom[i] = null;
                vistedNodes[i] = false;
                if (!(startNode is null) && nodes[i] == startNode)
                {
                    // The cost of going from start to start is zero.
                    gScore[i] = 0;
                    // For the first node, that value is completely heuristic.
                    fScore[i] = getDistance(startNode.getPosition(), endNode.getPosition());
                }
                else
                {
                    gScore[i] = float.MaxValue;
                    fScore[i] = float.MaxValue;
                }
            }

            while (openSet.Count > 0)
            {
                if (!isAlive)
                {
                    return null;
                }

                var currentPosition = findPositionOfSmallest(fScore, vistedNodes);
                if (currentPosition < 0 || currentPosition > nodes.Count)
                {
                    break;
                }

                var current = nodes[currentPosition];
                if (current == endNode) return reconstruct_path(cameFrom, current);

                openSet.Remove(current);
                vistedNodes[currentPosition] = true;
                foreach (var neighbor in current.getNeighbors())
                {
                    if (endNode is null) continue;
                    var neighborPosition = findPositionInGrid(neighbor);
                    var isNeighborVisited = vistedNodes[neighborPosition];
                    if (isNeighborVisited) continue;
                    if (!isNodeInList(openSet, current)) openSet.Add(current);

                    var tentativeGScore = gScore[currentPosition] +
                                          getDistance(current.getPosition(), neighbor.getPosition());
                    if (tentativeGScore >= gScore[neighborPosition]) continue;
                    cameFrom[neighborPosition] = current;
                    gScore[neighborPosition] = tentativeGScore;
                    fScore[neighborPosition] =
                        tentativeGScore + getDistance(neighbor.getPosition(), endNode.getPosition());
                }
            }

            if (startNode is null) throw new Exception("A* couldn't find the path, start node was null");
            if (endNode is null) throw new Exception("A* couldn't find the path, end node was null");
            Debug.Log(
                $"A* couldn't find the path between {startNode.getPosition()} and {endNode.getPosition()}");
            throw new Exception(
                $"A* couldn't find the path between {startNode.getPosition()} and {endNode.getPosition()}");
        }

        private List<Node> reconstruct_path(IReadOnlyList<Node> cameFrom, Node current)
        {
            var totalPath = new List<Node> {current};
            while (cameFrom[findPositionInGrid(current)] != null)
            {
                if (current is null) continue;
                current = cameFrom[findPositionInGrid(current)];
                var temp = new List<Node> {current};
                temp.AddRange(totalPath);
                totalPath = temp;
            }

            return totalPath;
        }

        private static int findPositionOfSmallest(IReadOnlyList<float> fScore, IReadOnlyList<bool> visitedNodes)
        {
            var smallestYet = float.MaxValue;
            var positionOfSmallest = -1;
            for (var i = 0; i < fScore.Count; i++)
            {
                if (visitedNodes[i] || !(fScore[i] < smallestYet)) continue;
                smallestYet = fScore[i];
                positionOfSmallest = i;
            }

            return positionOfSmallest;
        }

        private static float getDistance(Vector3 nodeA, Vector3 nodeB)
        {
            var direction = nodeA - nodeB;
            return Mathf.Sqrt(Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2) + Mathf.Pow(direction.z, 2));
        }

        // ReSharper restore Unity.ExpensiveCode

        private static bool isNodeInList(IEnumerable<Node> nodes, Node node)
        {
            return nodes.Any(t => t == node);
        }

        private int findPositionInGrid(Node node)
        {
            for (var i = 0; i < nodes.Count; i++)
                if (nodes[i] == node)
                    return i;

            throw new Exception($"the given node ({node.getPosition()}) isn't in the grid");
        }
    }
}