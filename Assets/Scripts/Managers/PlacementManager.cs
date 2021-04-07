﻿using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField] private GameObject roomUI;
        [SerializeField] private ResourcesManager rm;
        [SerializeField] private GameObject waiter;
        [SerializeField] private List<Node> nodes;


        public int Price { get; set; }
        public int Reputation { get; set; }

        public void CreateBlueprint(GameObject blueprint)
        {
            if (rm.Gold < Price) return;
        
            Instantiate(blueprint);
            rm.Gold -= Price;
            rm.Reputation += Reputation;
            roomUI.gameObject.SetActive(!roomUI.gameObject.activeSelf);
        }

        public void AddWaiter(GameObject btn)
        {
            if (rm.Gold < Price) return;
            
            rm.Gold -= Price;
            rm.Reputation += Reputation;
            waiter.SetActive(true);
            btn.SetActive(false);
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