using System;
using System.Collections.Generic;
using Interface;
using Managers;
using Prototypes.Pathfinding.Scripts;
using UnityEngine;

namespace Environnement
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField] private GameObject roomUI;
        [SerializeField] private ResourcesManager rm;

        [SerializeField] private List<Node> nodes;


        public int Price { get; set; }

        public void CreateBlueprint(GameObject blueprint)
        {
            if (rm.Gold < Convert.ToUInt32(Price)) return;
        
            Instantiate(blueprint);
            rm.Gold -= Convert.ToUInt32(Price);
            roomUI.gameObject.SetActive(!roomUI.gameObject.activeSelf);
        }

        public void InitAllNodes()
        {
            foreach (var node in nodes)
            {
                node.initialize();
            }
        }
    }
}