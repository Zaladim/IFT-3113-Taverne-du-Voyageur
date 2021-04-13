using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Managers
{
    public class PlacementManager : MonoBehaviour
    {
        [SerializeField] private GameObject roomUI;
        [SerializeField] private ResourcesManager rm;
        [SerializeField] private List<Node> nodes;
        private GameManager gameManager;

        public int Price { get; set; }
        public int Reputation { get; set; }

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        public void CreateBlueprint(GameObject blueprint)
        {
            if (rm.Gold < Price)
            {
                gameManager.ToggleGamePaused();
                return;
            }

            Instantiate(blueprint);
            rm.Gold -= Price;
            rm.Reputation += Reputation;
            roomUI.gameObject.SetActive(!roomUI.gameObject.activeSelf);
        }

        public void InitAllNodes()
        {
            foreach (var node in nodes) node.initialize();
        }
    }
}