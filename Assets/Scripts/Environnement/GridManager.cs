using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Grid grid;

    public Grid Grid => grid;

    private void Awake()
    {
        grid = new Grid(150, 150, 0.5f,new Vector3(-35.25f, 2f, -25f));
    }
    
    public Vector3 GetWorldPosition(int x, int y)
    {
        return Grid.GetWorldPosition(x, y);
    }

    public void getXY(Vector3 worldPosition, out int x, out int y)
    {
        grid.getXY(worldPosition, out x, out y);
    }

    public Vector3 getCellPosition(Vector3 worldPosition)
    {
        return grid.getCellPosition(worldPosition);
    }

    public void SetValue(int x, int y, int value)
    {
        grid.SetValue(x, y, value);
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        grid.SetValue(worldPosition, value);
    }

    public int GetValue(int x, int y)
    {
        return grid.GetValue(x, y);
    }
    
    public int GetValue(Vector3 worldPosition)
    {
        return grid.GetValue(worldPosition);
    }
    
    void OnDrawGizmosSelected()
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.GetValue(x, y) == 0)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(grid.GetWorldPosition(x, y), new Vector3(0.1f, 0, 0.1f));
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(grid.GetWorldPosition(x, y), new Vector3(0.5f, 0, 0.5f));
                }
            }
        }
    }
    
}
