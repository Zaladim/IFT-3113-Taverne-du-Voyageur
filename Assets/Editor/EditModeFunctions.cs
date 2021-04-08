using Pathfinding;
using UnityEngine;
using UnityEditor;

public class EditModeFunctions : EditorWindow
{
    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("Edit Mode Functions");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Run Function"))
        {
            GameObject graph = GameObject.Find("Graph");
            Graph Nodes = graph.GetComponent<Graph>();
            Nodes.drawGraph(true);
            Debug.Log("The function ran.");
        }
    }
}