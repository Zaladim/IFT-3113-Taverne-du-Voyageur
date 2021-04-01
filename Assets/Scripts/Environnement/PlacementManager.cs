using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using Prototypes.Pathfinding.Scripts;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject roomUI;
    [SerializeField] private RessourcesManager rm;

    [SerializeField] private List<Node> nodes;


    private int price;

    public int Price
    {
        get => price;
        set => price = value;
    }

    public void CreateBlueprint(GameObject blueprint)
    {
        if (rm.Gold >= Convert.ToUInt32(price))
        {
            Instantiate(blueprint);
            rm.Gold -= Convert.ToUInt32(price);
            if (roomUI.gameObject.activeSelf)
            {
                roomUI.gameObject.SetActive(false);
            }
            else
            {
                roomUI.gameObject.SetActive(true);
            }
        }
    }
    

    public void InitAllNodes()
    {
        foreach (var node in nodes)
        {
            node.initialize();
        }
    }
}