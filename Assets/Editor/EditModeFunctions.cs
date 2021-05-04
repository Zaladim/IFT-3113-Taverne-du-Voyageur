using Pathfinding;
using UnityEngine;
using UnityEditor;
using System;

public class EditModeFunctions : EditorWindow
{
    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("Edit Mode Functions");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Show graph"))
        {
            GameObject graph = GameObject.Find("Graph");
            Graph Nodes = graph.GetComponent<Graph>();
            Nodes.drawGraph(Application.isEditor);
            Debug.Log("Graph is being shown");
        }
        if (GUILayout.Button("Update Graph"))
        {
            if (!Application.isPlaying)
            {
                Debug.Log("the game isn't running");
            }
            else
            {
                GameObject graph = GameObject.Find("Graph");
                Graph[] Nodes = graph.GetComponents<Graph>();
                if (Nodes.Length > 1)
                {
                    throw new Exception("there is more than one graph in the gameobject Graph");
                }
                else
                {
                    Nodes[0].UpdateGraph();
                    Debug.Log("The graph was updated");
                }
            }
        }
    }
}