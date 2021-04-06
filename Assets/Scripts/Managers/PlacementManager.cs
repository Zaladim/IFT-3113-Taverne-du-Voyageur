using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Environnement
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField] private GameObject roomUI;
        [SerializeField] private ResourcesManager rm;
        [SerializeField] private GameObject waiter;
        [SerializeField] private List<Node> nodes;


        public int Price { get; set; }
        public int Reputation { get; set; }
        public int Seats { get; set; }

        public void CreateBlueprint(GameObject blueprint)
        {
            if (rm.Gold < Convert.ToUInt32(Price)) return;
        
            Instantiate(blueprint);
            rm.Gold -= Convert.ToUInt32(Price);
            rm.Reputation = Reputation;
            roomUI.gameObject.SetActive(!roomUI.gameObject.activeSelf);
        }

        public void AddWaiter(GameObject btn)
        {
            if (rm.Gold < Convert.ToUInt32(Price)) return;
            
            rm.Gold -= Convert.ToUInt32(Price);
            rm.Reputation = Reputation;
            rm.Seats = Seats;
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